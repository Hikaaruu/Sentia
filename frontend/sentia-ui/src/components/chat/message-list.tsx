import { useEffect, useRef } from "react";
import { useMessages } from "@/hooks/use-messages";
import { useMarkAsRead } from "@/hooks/use-chats";
import { useChats } from "@/hooks/use-chats";
import { useAuthStore } from "@/stores/auth.store";
import { MessageBubble } from "./message-bubble";
import { Skeleton } from "@/components/ui/skeleton";
import { parseUtcDate } from "@/lib/utils";

function isSameDay(a: string, b: string) {
  const da = parseUtcDate(a);
  const db = parseUtcDate(b);
  return (
    da.getFullYear() === db.getFullYear() &&
    da.getMonth() === db.getMonth() &&
    da.getDate() === db.getDate()
  );
}

function formatDateLabel(iso: string) {
  const d = parseUtcDate(iso);
  const today = new Date();
  const yesterday = new Date(today);
  yesterday.setDate(today.getDate() - 1);

  if (isSameDay(iso, today.toISOString())) return "Today";
  if (isSameDay(iso, yesterday.toISOString())) return "Yesterday";
  return d.toLocaleDateString([], {
    month: "long",
    day: "numeric",
    year: "numeric",
  });
}

interface MessageListProps {
  chatId: number;
}

export function MessageList({ chatId }: MessageListProps) {
  const userId = useAuthStore((s) => s.user?.userId);
  const { data, isFetchingPreviousPage, hasPreviousPage, fetchPreviousPage } =
    useMessages(chatId);
  const { mutate: markAsRead } = useMarkAsRead(chatId);
  const { data: chats } = useChats();

  const topSentinelRef = useRef<HTMLDivElement>(null);
  const bottomRef = useRef<HTMLDivElement>(null);
  const scrollContainerRef = useRef<HTMLDivElement>(null);

  const chat = chats?.find((c) => c.chatId === chatId);
  const otherLastReadId = chat?.otherParticipantLastReadMessageId ?? null;

  const lastMessageIdRef = useRef<string | null>(null);
  const isInitialLoad = useRef(true);

  useEffect(() => {
    isInitialLoad.current = true;
    lastMessageIdRef.current = null;
    lastReadSentRef.current = null;
  }, [chatId]);

  useEffect(() => {
    const messages = data?.messages ?? [];
    if (messages.length === 0) return;

    const lastMsg = messages[messages.length - 1];

    if (isInitialLoad.current) {
      if (scrollContainerRef.current) {
        scrollContainerRef.current.scrollTop =
          scrollContainerRef.current.scrollHeight;
      }
      isInitialLoad.current = false;
    } else if (lastMsg.id !== lastMessageIdRef.current) {
      if (scrollContainerRef.current) {
        scrollContainerRef.current.scrollTop =
          scrollContainerRef.current.scrollHeight;
      }
    }

    lastMessageIdRef.current = lastMsg.id;
  }, [data?.messages]);

  useEffect(() => {
    const sentinel = topSentinelRef.current;
    if (!sentinel) return;

    const observer = new IntersectionObserver(
      (entries) => {
        if (
          entries[0]?.isIntersecting &&
          hasPreviousPage &&
          !isFetchingPreviousPage
        ) {
          fetchPreviousPage();
        }
      },
      { threshold: 0 },
    );

    observer.observe(sentinel);
    return () => observer.disconnect();
  }, [hasPreviousPage, isFetchingPreviousPage, fetchPreviousPage]);

  const markReadRef = useRef(markAsRead);
  markReadRef.current = markAsRead;

  const lastReadSentRef = useRef<string | null>(null);

  useEffect(() => {
    const messages = data?.messages;
    if (!messages?.length) return;
    const lastOtherMsg = messages.findLast(
      (msg) => msg.senderId !== userId && msg.chatId === chatId,
    );
    if (lastOtherMsg && lastReadSentRef.current !== lastOtherMsg.id) {
      lastReadSentRef.current = lastOtherMsg.id;
      markReadRef.current({ messageId: lastOtherMsg.id });
    }
  }, [data?.messages, userId, chatId]);

  const messages = data?.messages ?? [];

  return (
    <div
      ref={scrollContainerRef}
      className="flex-1 overflow-y-auto px-4 py-4"
      style={{ overflowAnchor: "auto" } as React.CSSProperties}
    >
      <div
        ref={topSentinelRef}
        style={{ overflowAnchor: "none" } as React.CSSProperties}
      />

      {isFetchingPreviousPage && (
        <div className="flex flex-col gap-2 pb-4">
          {Array.from({ length: 3 }).map((_, i) => (
            <div
              key={i}
              className={`flex ${i % 2 === 0 ? "justify-start" : "justify-end"}`}
            >
              <Skeleton className="h-10 w-48 rounded-2xl" />
            </div>
          ))}
        </div>
      )}

      <div className="flex flex-col gap-1">
        {messages.map((message, idx) => {
          const prevMessage = messages[idx - 1];
          const showDateSeparator =
            !prevMessage ||
            !isSameDay(prevMessage.createdAt, message.createdAt);

          const isOwn = message.senderId === userId;
          const isRead =
            isOwn && otherLastReadId !== null && message.id <= otherLastReadId;

          return (
            <div key={message.id}>
              {showDateSeparator && (
                <div className="my-3 flex items-center gap-3">
                  <div className="flex-1 border-t border-border" />
                  <span className="text-xs text-muted-foreground">
                    {formatDateLabel(message.createdAt)}
                  </span>
                  <div className="flex-1 border-t border-border" />
                </div>
              )}
              <MessageBubble message={message} isOwn={isOwn} isRead={isRead} />
            </div>
          );
        })}
      </div>

      <div
        ref={bottomRef}
        style={{ overflowAnchor: "none" } as React.CSSProperties}
      />
    </div>
  );
}
