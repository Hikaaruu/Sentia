import type {
  ChatSummaryDto,
  CreateChatRequest,
  CreateOrGetPrivateChatResult,
  MarkAsReadRequest,
} from "./types";
import { apiClient } from "./client";

export async function getChats(): Promise<ChatSummaryDto[]> {
  const res = await apiClient.get<ChatSummaryDto[]>("/api/chats");
  return res.data;
}

export async function createChat(
  data: CreateChatRequest,
): Promise<CreateOrGetPrivateChatResult> {
  const res = await apiClient.post<CreateOrGetPrivateChatResult>(
    "/api/chats",
    data,
  );
  return res.data;
}

export async function markChatAsRead(
  chatId: number,
  data: MarkAsReadRequest,
): Promise<void> {
  await apiClient.post(`/api/chats/${chatId}/read`, data);
}
