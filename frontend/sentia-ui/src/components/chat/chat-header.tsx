import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import type { ChatSummaryDto } from "@/api/types";

interface ChatHeaderProps {
  chat: ChatSummaryDto;
}

export function ChatHeader({ chat }: ChatHeaderProps) {
  const initials = chat.otherParticipantUsername.slice(0, 2).toUpperCase();

  return (
    <div className="flex h-14 items-center gap-3 border-b border-border bg-background px-4">
      <Avatar className="h-8 w-8">
        <AvatarFallback className="text-xs">{initials}</AvatarFallback>
      </Avatar>
      <div className="flex flex-col">
        <span className="text-sm font-medium leading-tight">
          {chat.otherParticipantUsername}
        </span>
      </div>
    </div>
  );
}
