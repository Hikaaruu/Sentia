import { useEffect, useRef, useCallback } from "react";
import {
  HubConnectionBuilder,
  HubConnectionState,
  LogLevel,
} from "@microsoft/signalr";
import { useQueryClient } from "@tanstack/react-query";
import { useAuthStore } from "@/stores/auth.store";
import { useChatStore } from "@/stores/chat.store";
import { useSignalRStore } from "@/stores/signalr.store";
import type {
  NewMessagePayload,
  SentimentUpdatePayload,
  ReadReceiptPayload,
  TypingPayload,
  MessageDto,
  ChatSummaryDto,
} from "@/api/types";

export function useSignalR() {
  const queryClient = useQueryClient();
  const setTyping = useChatStore((s) => s.setTyping);
  const setStatus = useSignalRStore((s) => s.setStatus);
  const connectionRef = useRef<ReturnType<typeof buildConnection> | null>(null);

  function buildConnection() {
    return new HubConnectionBuilder()
      .withUrl(import.meta.env.VITE_HUB_URL as string, {
        accessTokenFactory: () => useAuthStore.getState().token ?? "",
      })
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Warning)
      .build();
  }

  useEffect(() => {
    const connection = buildConnection();
    connectionRef.current = connection;

    connection.on("ReceiveNewMessage", (payload: NewMessagePayload) => {
      const newMsg: MessageDto = {
        id: payload.messageId,
        chatId: payload.chatId,
        senderId: payload.senderId,
        content: payload.content,
        createdAt: payload.createdAt,
        sentimentScore: null,
        sentimentLabel: null,
      };
      // Append to the last page of the infinite message list
      queryClient.setQueryData<{
        pages: MessageDto[][];
        pageParams: unknown[];
      }>(["messages", payload.chatId], (old) => {
        if (!old) return old;
        const pages = old.pages.map((p, i) =>
          i === old.pages.length - 1 ? [...p, newMsg] : p,
        );
        return { ...old, pages };
      });

      // Update chat list summary
      queryClient.setQueryData<ChatSummaryDto[]>(["chats"], (old) => {
        if (!old) return old;
        return old.map((chat) =>
          chat.chatId === payload.chatId
            ? {
                ...chat,
                lastMessageAt: payload.createdAt,
                lastMessageContent: payload.content,
                lastMessageSenderId: payload.senderId,
                unreadCount:
                  payload.senderId !== useAuthStore.getState().user?.userId
                    ? chat.unreadCount + 1
                    : chat.unreadCount,
              }
            : chat,
        );
      });
    });

    connection.on(
      "UpdateMessageSentiment",
      (payload: SentimentUpdatePayload) => {
        queryClient.setQueryData<{
          pages: MessageDto[][];
          pageParams: unknown[];
        }>(["messages", payload.chatId], (old) => {
          if (!old) return old;
          const pages = old.pages.map((page) =>
            page.map((msg) =>
              msg.id === payload.messageId
                ? {
                    ...msg,
                    sentimentLabel: payload.sentimentLabel,
                    sentimentScore: payload.sentimentScore,
                  }
                : msg,
            ),
          );
          return { ...old, pages };
        });
      },
    );

    connection.on("ReceiveReadReceipt", (payload: ReadReceiptPayload) => {
      queryClient.setQueryData<ChatSummaryDto[]>(["chats"], (old) => {
        if (!old) return old;
        return old.map((chat) =>
          chat.chatId === payload.chatId
            ? { ...chat, otherParticipantLastReadMessageId: payload.messageId }
            : chat,
        );
      });
    });

    connection.on("ReceiveTyping", (payload: TypingPayload) => {
      setTyping(payload.chatId, payload.senderId);
    });

    connection.onreconnecting(() => setStatus("connecting"));
    connection.onreconnected(() => setStatus("connected"));
    connection.onclose(() => setStatus("disconnected"));

    setStatus("connecting");
    connection
      .start()
      .then(() => setStatus("connected"))
      .catch(() => setStatus("disconnected"));

    return () => {
      connection.stop();
      connectionRef.current = null;
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const sendTyping = useCallback((chatId: number) => {
    const conn = connectionRef.current;
    if (conn?.state === HubConnectionState.Connected) {
      conn.invoke("SendTyping", chatId).catch(() => undefined);
    }
  }, []);

  return { sendTyping };
}
