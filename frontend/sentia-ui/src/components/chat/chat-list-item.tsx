import { NavLink } from "react-router-dom";
import { formatDistanceToNow } from "date-fns";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { Badge } from "@/components/ui/badge";
import { cn, parseUtcDate } from "@/lib/utils";
import type { ChatSummaryDto } from "@/api/types";
import { useAuthStore } from "@/stores/auth.store";
import { useChatStore } from "@/stores/chat.store";

interface ChatListItemProps {
  chat: ChatSummaryDto;
}

export function ChatListItem({ chat }: ChatListItemProps) {
  const userId = useAuthStore((s) => s.user?.userId);
  const isTyping = useChatStore((s) => !!s.typingUsers[chat.chatId]);
  const initials = chat.otherParticipantUsername.slice(0, 2).toUpperCase();

  let snippetText = "No messages yet";
  if (chat.lastMessageContent) {
    snippetText =
      (chat.lastMessageSenderId === userId ? "You: " : "") +
      chat.lastMessageContent.slice(0, 40) +
      (chat.lastMessageContent.length > 40 ? "…" : "");
  }

  const snippetNode = isTyping ? (
    <span className="font-medium italic text-primary animate-pulse">
      typing...
    </span>
  ) : (
    snippetText
  );

  const isNewChat = !chat.lastMessageContent;
  const timeAgo = isNewChat
    ? "New"
    : formatDistanceToNow(parseUtcDate(chat.lastMessageAt), {
        addSuffix: true,
      });

  return (
    <NavLink
      to={`/chats/${chat.chatId}`}
      className={({ isActive }) =>
        cn(
          "flex items-center gap-3 rounded-lg px-3 py-2.5 transition-colors hover:bg-muted/60",
          isActive && "bg-muted",
        )
      }
    >
      <Avatar className="h-9 w-9 shrink-0">
        <AvatarFallback className="text-xs">{initials}</AvatarFallback>
      </Avatar>

      <div className="flex min-w-0 flex-1 flex-col">
        <div className="flex items-baseline justify-between gap-1">
          <span className="truncate text-sm font-medium">
            {chat.otherParticipantUsername}
          </span>
          <span className="shrink-0 text-[10px] text-muted-foreground">
            {timeAgo}
          </span>
        </div>
        <div className="flex items-center justify-between gap-1">
          <span className="truncate text-xs text-muted-foreground">
            {snippetNode}
          </span>
          {chat.unreadCount > 0 && (
            <Badge className="h-4 min-w-4 shrink-0 rounded-full px-1 text-[10px]">
              {chat.unreadCount > 99 ? "99+" : chat.unreadCount}
            </Badge>
          )}
        </div>
      </div>
    </NavLink>
  );
}
