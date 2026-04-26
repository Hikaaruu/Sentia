import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import type { ChatSummaryDto } from "@/api/types";
import { useChatStore } from "@/stores/chat.store";

interface ChatHeaderProps {
  chat: ChatSummaryDto;
}

export function ChatHeader({ chat }: ChatHeaderProps) {
  const initials = chat.otherParticipantUsername.slice(0, 2).toUpperCase();
  const isTyping = useChatStore((s) => !!s.typingUsers[chat.chatId]);

  return (
    <div className="flex h-14 items-center gap-3 border-b border-border bg-background px-4">
      <Avatar className="h-8 w-8">
        <AvatarFallback className="text-xs">{initials}</AvatarFallback>
      </Avatar>
      <div className="flex flex-col">
        <span className="text-sm font-medium leading-tight">
          {chat.otherParticipantUsername}
        </span>
        {isTyping && (
          <span className="text-[10px] font-medium text-primary animate-pulse">
            typing...
          </span>
        )}
      </div>
    </div>
  );
}
