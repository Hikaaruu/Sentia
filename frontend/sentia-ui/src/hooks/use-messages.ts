import {
  useInfiniteQuery,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";
import { toast } from "sonner";
import { getMessages, sendMessage } from "@/api/messages";
import type { MessageDto } from "@/api/types";
import { useAuthStore } from "@/stores/auth.store";

const PAGE_SIZE = 30;

export function useMessages(chatId: number) {
  return useInfiniteQuery({
    queryKey: ["messages", chatId],
    queryFn: ({ pageParam }) =>
      getMessages({
        chatId,
        before: pageParam as string | undefined,
        take: PAGE_SIZE,
      }),
    initialPageParam: undefined as string | undefined,
    getPreviousPageParam: (firstPage) => {
      if (firstPage.length < PAGE_SIZE) return undefined;
      return firstPage[0]?.id;
    },
    getNextPageParam: () => undefined,
    select: (data) => ({
      ...data,
      // Flatten oldest→newest for rendering
      messages: data.pages.flat(),
    }),
  });
}

interface SendMessageVariables {
  messageId: string;
  content: string;
}

export function useSendMessage(chatId: number) {
  const queryClient = useQueryClient();
  const user = useAuthStore((s) => s.user);

  return useMutation({
    mutationFn: ({ messageId, content }: SendMessageVariables) => {
      return sendMessage(chatId, { messageId, content });
    },
    onMutate: async ({ messageId, content }) => {
      await queryClient.cancelQueries({ queryKey: ["messages", chatId] });

      const snapshot = queryClient.getQueryData(["messages", chatId]);

      const optimistic: MessageDto = {
        id: messageId,
        chatId,
        senderId: user?.userId ?? "",
        content,
        createdAt: new Date().toISOString(),
        sentimentScore: null,
        sentimentLabel: null,
      };

      queryClient.setQueryData<{
        pages: MessageDto[][];
        pageParams: unknown[];
      }>(["messages", chatId], (old) => {
        if (!old) return old;
        const pages = old.pages.map((p, i) =>
          i === old.pages.length - 1 ? [...p, optimistic] : p,
        );
        return { ...old, pages };
      });

      return { snapshot, optimisticId: messageId };
    },
    onError(_err, _vars, context) {
      if (context?.snapshot) {
        queryClient.setQueryData(["messages", chatId], context.snapshot);
      }
      toast.error("Failed to send message");
    },
    onSuccess(_data, _vars, context) {
      // Replace optimistic message id with confirmed id from server
      if (!context) return;
      queryClient.setQueryData<{
        pages: MessageDto[][];
        pageParams: unknown[];
      }>(["messages", chatId], (old) => {
        if (!old) return old;
        const pages = old.pages.map((page) =>
          page.map((msg) =>
            msg.id === context.optimisticId
              ? { ...msg, id: _data.messageId }
              : msg,
          ),
        );
        return { ...old, pages };
      });
    },
  });
}
