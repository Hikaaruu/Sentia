import type {
  MessageDto,
  SendMessageRequest,
  SendMessageResponse,
} from "./types";
import { apiClient } from "./client";

export interface GetMessagesParams {
  chatId: number;
  before?: string;
  take?: number;
}

export async function getMessages({
  chatId,
  before,
  take = 30,
}: GetMessagesParams): Promise<MessageDto[]> {
  const res = await apiClient.get<MessageDto[]>(
    `/api/chats/${chatId}/messages`,
    {
      params: { before, take },
    },
  );
  return res.data;
}

export async function sendMessage(
  chatId: number,
  data: SendMessageRequest,
): Promise<SendMessageResponse> {
  const res = await apiClient.post<SendMessageResponse>(
    `/api/chats/${chatId}/messages`,
    data,
  );
  return res.data;
}
