import { useEffect, useRef, useState } from "react";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { Skeleton } from "@/components/ui/skeleton";
import { Button } from "@/components/ui/button";
import { useUsers } from "@/hooks/use-users";
import { useChats, useCreateChat } from "@/hooks/use-chats";
import type { UserDto } from "@/api/types";
import { useAuthStore } from "@/stores/auth.store";

interface NewChatModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export function NewChatModal({ open, onOpenChange }: NewChatModalProps) {
  const userId = useAuthStore((s) => s.user?.userId);
  const { data: chats } = useChats();
  const { data, isFetchingNextPage, hasNextPage, fetchNextPage, isLoading } =
    useUsers();
  const { mutate: createChat, isPending } = useCreateChat();
  const [creatingFor, setCreatingFor] = useState<string | null>(null);

  const bottomRef = useRef<HTMLDivElement>(null);

  // Infinite scroll sentinel
  useEffect(() => {
    const el = bottomRef.current;
    if (!el) return;
    const observer = new IntersectionObserver(
      (entries) => {
        if (entries[0]?.isIntersecting && hasNextPage && !isFetchingNextPage) {
          fetchNextPage();
        }
      },
      { threshold: 0 },
    );
    observer.observe(el);
    return () => observer.disconnect();
  }, [hasNextPage, isFetchingNextPage, fetchNextPage]);

  const existingChatUserIds = new Set(
    chats?.map((c) => c.otherParticipantId) ?? [],
  );
  const allUsers: UserDto[] = data?.pages.flatMap((p) => p.items) ?? [];
  // Exclude self
  const users = allUsers.filter((u) => u.id !== userId);

  function handleSelect(user: UserDto) {
    if (isPending) return;
    setCreatingFor(user.id);
    createChat(
      { recipientUserId: user.id },
      {
        onSettled: () => {
          setCreatingFor(null);
          onOpenChange(false);
        },
      },
    );
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-h-[70vh] flex flex-col gap-0 p-0">
        <DialogHeader className="px-4 py-3 border-b border-border">
          <DialogTitle className="text-sm font-semibold">
            New conversation
          </DialogTitle>
        </DialogHeader>

        <div className="flex-1 overflow-y-auto">
          {isLoading && (
            <div className="flex flex-col gap-1 p-3">
              {Array.from({ length: 6 }).map((_, i) => (
                <div key={i} className="flex items-center gap-3 px-2 py-2">
                  <Skeleton className="h-8 w-8 rounded-full" />
                  <Skeleton className="h-3 w-32" />
                </div>
              ))}
            </div>
          )}

          {!isLoading && users.length === 0 && (
            <p className="px-4 py-6 text-center text-xs text-muted-foreground">
              No other users found.
            </p>
          )}

          <div className="flex flex-col gap-0.5 p-2">
            {users.map((user) => {
              const hasChat = existingChatUserIds.has(user.id);
              const isCreating = creatingFor === user.id;

              return (
                <div
                  key={user.id}
                  className="flex items-center gap-3 rounded-lg px-3 py-2"
                >
                  <Avatar className="h-8 w-8 shrink-0">
                    <AvatarFallback className="text-xs">
                      {user.userName.slice(0, 2).toUpperCase()}
                    </AvatarFallback>
                  </Avatar>
                  <span className="flex-1 text-sm font-medium">
                    {user.userName}
                  </span>
                  {hasChat ? (
                    <span className="text-xs text-muted-foreground">
                      Existing
                    </span>
                  ) : (
                    <Button
                      size="sm"
                      variant="outline"
                      className="h-7 text-xs"
                      disabled={isPending}
                      onClick={() => handleSelect(user)}
                    >
                      {isCreating ? "Opening…" : "Message"}
                    </Button>
                  )}
                </div>
              );
            })}
          </div>

          <div ref={bottomRef} className="h-1" />
          {isFetchingNextPage && (
            <div className="flex justify-center py-3">
              <Skeleton className="h-3 w-16" />
            </div>
          )}
        </div>
      </DialogContent>
    </Dialog>
  );
}
