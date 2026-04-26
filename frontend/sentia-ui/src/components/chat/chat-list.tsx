import { ChatListItem } from "./chat-list-item";
import { Skeleton } from "@/components/ui/skeleton";
import type { ChatSummaryDto } from "@/api/types";

interface ChatListProps {
  chats: ChatSummaryDto[] | undefined;
  isLoading: boolean;
}

export function ChatList({ chats, isLoading }: ChatListProps) {
  if (isLoading) {
    return (
      <div className="flex flex-col gap-1 p-2">
        {Array.from({ length: 5 }).map((_, i) => (
          <div
            key={i}
            className="flex items-center gap-3 rounded-lg px-3 py-2.5"
          >
            <Skeleton className="h-9 w-9 rounded-full shrink-0" />
            <div className="flex flex-1 flex-col gap-1.5">
              <Skeleton className="h-3 w-24" />
              <Skeleton className="h-2.5 w-36" />
            </div>
          </div>
        ))}
      </div>
    );
  }

  if (!chats?.length) {
    return (
      <p className="px-4 py-6 text-center text-xs text-muted-foreground">
        No conversations yet.
      </p>
    );
  }

  return (
    <div className="flex flex-col gap-0.5 p-2">
      {chats.map((chat) => (
        <ChatListItem key={chat.chatId} chat={chat} />
      ))}
    </div>
  );
}
