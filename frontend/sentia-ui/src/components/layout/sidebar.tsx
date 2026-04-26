import { useState } from "react";
import { PenSquare, LogOut } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Separator } from "@/components/ui/separator";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { ChatList } from "@/components/chat/chat-list";
import { NewChatModal } from "@/components/chat/new-chat-modal";
import { useChats } from "@/hooks/use-chats";
import { useAuthStore } from "@/stores/auth.store";
import { useLogout } from "@/hooks/use-logout";

export function Sidebar() {
  const [modalOpen, setModalOpen] = useState(false);
  const { data: chats, isLoading } = useChats();
  const user = useAuthStore((s) => s.user);
  const logout = useLogout();

  const initials = user?.username.slice(0, 2).toUpperCase() ?? "?";

  return (
    <>
      <aside className="flex h-full w-72 shrink-0 flex-col border-r border-border bg-background">
        {/* Header */}
        <div className="flex h-14 items-center justify-between px-4">
          <span className="text-base font-semibold tracking-tight">Sentia</span>
          <Button
            size="icon"
            variant="ghost"
            className="h-8 w-8"
            onClick={() => setModalOpen(true)}
            aria-label="New conversation"
          >
            <PenSquare className="h-4 w-4" />
          </Button>
        </div>

        <Separator />

        {/* Chat list */}
        <div className="flex-1 overflow-y-auto">
          <ChatList chats={chats} isLoading={isLoading} />
        </div>

        <Separator />

        {/* Footer */}
        <div className="flex items-center gap-3 px-4 py-3">
          <Avatar className="h-7 w-7 shrink-0">
            <AvatarFallback className="text-[10px]">{initials}</AvatarFallback>
          </Avatar>
          <span className="flex-1 truncate text-xs font-medium">
            {user?.username}
          </span>
          <Button
            size="icon"
            variant="ghost"
            className="h-7 w-7 text-muted-foreground"
            onClick={logout}
            aria-label="Sign out"
          >
            <LogOut className="h-3.5 w-3.5" />
          </Button>
        </div>
      </aside>

      <NewChatModal open={modalOpen} onOpenChange={setModalOpen} />
    </>
  );
}
