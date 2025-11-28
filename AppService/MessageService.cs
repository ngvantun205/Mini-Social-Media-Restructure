using Microsoft.AspNetCore.SignalR;

namespace Mini_Social_Media.AppService {
    public class MessageService : IMessageService {
        private readonly IMessageRepository _messageRepo;
        private readonly IUserRepository _userRepo;
        private readonly IHubContext<ChatHub> _hub;

        public MessageService(IMessageRepository msgRepo, IUserRepository userRepo, IHubContext<ChatHub> hub) {
            _messageRepo = msgRepo;
            _userRepo = userRepo;
            _hub = hub;
        }

        public async Task<MessageViewModel> SendMessageAsync(int senderId, int receiverId, string content) {
            var conversation = await _messageRepo.GetConversationAsync(senderId, receiverId);

            if (conversation == null) {
                conversation = new Conversations {
                    User1Id = Math.Min(senderId, receiverId),
                    User2Id = Math.Max(senderId, receiverId),
                    UpdatedAt = DateTime.UtcNow
                };

                await _messageRepo.CreateConversationAsync(conversation);
            }

            var message = new Messages {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = content,
                CreatedAt = DateTime.UtcNow,
                IsRead = false,
                ConversationId = conversation.Id
            };

            await _messageRepo.AddMessageAsync(message);

            conversation.UpdatedAt = DateTime.UtcNow;
            conversation.LatestMessageId = message.Id;
            await _messageRepo.UpdateConversationAsync(conversation);

            var sender = await _userRepo.GetByIdAsync(senderId);

            var dto = new MessageViewModel {
                Id = message.Id,
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = content,
                CreatedAt = message.CreatedAt,
                IsRead = false,
                SenderName = sender?.UserName ?? "Unknown",
                SenderAvatar = sender?.AvatarUrl ?? "/images/default-avatar.png"
            };

            await _hub.Clients.User(receiverId.ToString())
                .SendAsync("ReceiveMessage", dto);

            return dto;
        }

        public async Task<IEnumerable<ConversationViewModel>> GetUserConversationsAsync(int userId) {
            var convs = await _messageRepo.GetUserConversationsAsync(userId);
            var list = new List<ConversationViewModel>();

            foreach (var c in convs) {
                var partner = c.User1Id == userId ? c.User2 : c.User1;

                list.Add(new ConversationViewModel {
                    ConversationId = c.Id,
                    PartnerId = partner.Id,
                    PartnerName = partner.UserName,
                    PartnerAvatar = partner.AvatarUrl ?? "/images/default-avatar.png",
                    LastMessage = c.LatestMessage?.Content ?? "",
                    LastMessageTime = c.UpdatedAt,
                    IsRead = c.LatestMessage == null ||
                             c.LatestMessage.SenderId == userId ||
                             c.LatestMessage.IsRead
                });
            }

            return list;
        }

        public async Task<ConversationViewModel?> GetOrCreateConversationAsync(int currentUserId, int partnerId) {
            var c = await _messageRepo.GetConversationAsync(currentUserId, partnerId);

            if (c == null) {
                var newConv = new Conversations {
                    User1Id = Math.Min(currentUserId, partnerId),
                    User2Id = Math.Max(currentUserId, partnerId),
                    UpdatedAt = DateTime.UtcNow
                };

                await _messageRepo.CreateConversationAsync(newConv);

                var partner = await _userRepo.GetByIdAsync(partnerId);

                return new ConversationViewModel {
                    ConversationId = newConv.Id,
                    PartnerId = partnerId,
                    PartnerName = partner?.UserName ?? "Unknown",
                    PartnerAvatar = partner?.AvatarUrl ?? "/images/default-avatar.png",
                    LastMessage = "",
                    LastMessageTime = newConv.UpdatedAt,
                    IsRead = true
                };
            }

            var p = (c.User1Id == currentUserId) ? c.User2 : c.User1;

            return new ConversationViewModel {
                ConversationId = c.Id,
                PartnerId = p.Id,
                PartnerName = p.UserName,
                PartnerAvatar = p.AvatarUrl ?? "/images/default-avatar.png",
                LastMessage = c.LatestMessage?.Content ?? "",
                LastMessageTime = c.UpdatedAt,
                IsRead = c.LatestMessage == null ||
                         c.LatestMessage.SenderId == currentUserId ||
                         c.LatestMessage.IsRead
            };
        }

        public async Task<IEnumerable<MessageViewModel>> GetMessageHistoryAsync(int currentUserId, int partnerId) {
            var conv = await _messageRepo.GetConversationAsync(currentUserId, partnerId);
            if (conv == null)
                return new List<MessageViewModel>();

            var messages = await _messageRepo.GetMessagesAsync(conv.Id);

            return messages.Select(m => new MessageViewModel {
                Id = m.Id,
                SenderId = m.SenderId,
                ReceiverId = m.ReceiverId,
                Content = m.Content,
                CreatedAt = m.CreatedAt,
                IsRead = true,
                SenderName = m.Sender.UserName,
                SenderAvatar = m.Sender.AvatarUrl ?? "/images/default-avatar.png"
            });
        }
        public async Task MarkConversationAsReadAsync(int userId, int partnerId) {
            await _messageRepo.MarkAsRead(userId, partnerId);
        }

    }
}