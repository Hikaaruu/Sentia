import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { useNavigate } from "react-router-dom";
import { toast } from "sonner";
import { createChat, getChats, markChatAsRead } from "@/api/chats";
import type {
  ChatSummaryDto,
  CreateChatRequest,
  MarkAsReadRequest,
} from "@/api/types";

export function useChats() {
  return useQuery({
    queryKey: ["chats"],
    queryFn: getChats,
    staleTime: 30_000,
    select: (data) =>
      [...data].sort(
        (a, b) =>
          new Date(b.lastMessageAt).getTime() -
          new Date(a.lastMessageAt).getTime(),
      ),
  });
}

export function useCreateChat() {
  const queryClient = useQueryClient();
  const navigate = useNavigate();

  return useMutation({
    mutationFn: (data: CreateChatRequest) => createChat(data),
    onSuccess(result) {
      queryClient.invalidateQueries({ queryKey: ["chats"] });
      navigate(`/chats/${result.chatId}`);
    },
    onError() {
      toast.error("Could not start chat");
    },
  });
}

export function useMarkAsRead(chatId: number) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: MarkAsReadRequest) => markChatAsRead(chatId, data),
    onSuccess() {
      queryClient.setQueryData<ChatSummaryDto[]>(["chats"], (old) => {
        if (!old) return old;
        return old.map((chat) =>
          chat.chatId === chatId
            ? {
                ...chat,
                unreadCount: 0,
              }
            : chat,
        );
      });
      queryClient.invalidateQueries({ queryKey: ["chats"] });
    },
  });
}
