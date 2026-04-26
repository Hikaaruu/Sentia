import { useEffect, useRef } from "react";
import { useChatStore } from "@/stores/chat.store";

interface TypingIndicatorProps {
  chatId: number;
  username: string;
}

export function TypingIndicator({ chatId, username }: TypingIndicatorProps) {
  const typingUsers = useChatStore((s) => s.typingUsers);
  const clearTyping = useChatStore((s) => s.clearTyping);
  const timerRef = useRef<ReturnType<typeof setTimeout> | null>(null);

  const entry = typingUsers[chatId];

  useEffect(() => {
    if (!entry) return;

    if (timerRef.current) clearTimeout(timerRef.current);
    timerRef.current = setTimeout(() => clearTyping(chatId), 3000);

    return () => {
      if (timerRef.current) clearTimeout(timerRef.current);
    };
  }, [entry, chatId, clearTyping]);

  if (!entry) return null;

  return (
    <div className="flex items-center gap-2 px-4 py-1 text-xs text-muted-foreground">
      <span className="flex gap-0.5">
        <span className="animate-bounce [animation-delay:0ms]">•</span>
        <span className="animate-bounce [animation-delay:150ms]">•</span>
        <span className="animate-bounce [animation-delay:300ms]">•</span>
      </span>
      <span>{username} is typing…</span>
    </div>
  );
}
