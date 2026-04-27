export interface LoginRequest {
  username: string;
  password: string;
}

export interface RegisterRequest {
  username: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  userId: string;
  username: string;
}

export interface MeResponse {
  userId: string;
  username: string;
}

export const SentimentLabel = {
  Positive: 1,
  Neutral: 2,
  Negative: 3,
} as const;

export type SentimentLabel =
  (typeof SentimentLabel)[keyof typeof SentimentLabel];

export interface MessageDto {
  id: string;
  chatId: number;
  senderId: string;
  content: string;
  createdAt: string;
  sentimentScore: number | null;
  sentimentLabel: SentimentLabel | null;
}

export interface SendMessageRequest {
  messageId: string;
  content: string;
}

export interface SendMessageResponse {
  messageId: string;
}

export interface ChatSummaryDto {
  chatId: number;
  otherParticipantId: string;
  otherParticipantUsername: string;
  lastMessageAt: string;
  lastMessageContent: string | null;
  lastMessageSenderId: string | null;
  unreadCount: number;
  otherParticipantLastReadMessageId: string | null;
}

export interface CreateChatRequest {
  recipientUserId: string;
}

export interface CreateOrGetPrivateChatResult {
  chatId: number;
  isNew: boolean;
  otherParticipantId: string;
  otherParticipantUsername: string;
  otherParticipantLastReadMessageId: string | null;
  createdAt: string;
}

export interface MarkAsReadRequest {
  messageId: string;
}

export interface UserDto {
  id: string;
  userName: string;
}

export interface PaginatedResponse<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export interface NewMessagePayload {
  messageId: string;
  chatId: number;
  senderId: string;
  content: string;
  createdAt: string;
}

export interface SentimentUpdatePayload {
  messageId: string;
  chatId: number;
  sentimentLabel: SentimentLabel;
  sentimentScore: number;
}

export interface ReadReceiptPayload {
  messageId: string;
  chatId: number;
  readByUserId: string;
}

export interface TypingPayload {
  chatId: number;
  senderId: string;
}
