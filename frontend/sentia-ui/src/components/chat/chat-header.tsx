import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import type { ChatSummaryDto } from "@/api/types";
import { useChatStore } from "@/stores/chat.store";
import { TypingDots } from "./typing-dots";
import { ChevronLeft } from "lucide-react";
import { useNavigate } from "react-router-dom";

interface ChatHeaderProps {
  chat: ChatSummaryDto;
}

export function ChatHeader({ chat }: ChatHeaderProps) {
  const initials = chat.otherParticipantUsername.slice(0, 2).toUpperCase();
  const isTyping = useChatStore((s) => !!s.typingUsers[chat.chatId]);
  const navigate = useNavigate();

  return (
    <div className="flex h-14 items-center gap-3 border-b border-border bg-background px-4">
      <button
        type="button"
        onClick={() => navigate("/chats")}
        className="md:hidden -ml-1 mr-1 flex h-8 w-8 shrink-0 items-center justify-center rounded-md hover:bg-muted/60 transition-colors"
        aria-label="Back to chats"
      >
        <ChevronLeft className="h-4 w-4" />
      </button>
      <Avatar className="h-8 w-8 shrink-0">
        <AvatarFallback className="text-xs">{initials}</AvatarFallback>
      </Avatar>
      <div className="flex min-w-0 items-center gap-2">
        <span className="truncate text-sm font-medium leading-tight">
          {chat.otherParticipantUsername}
        </span>
        {isTyping && (
          <span className="flex items-center gap-0.5 text-[10px] font-medium italic text-primary mt-0.5">
            <TypingDots />
          </span>
        )}
      </div>
    </div>
  );
}
