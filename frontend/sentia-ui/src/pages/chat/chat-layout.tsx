import { Outlet, useParams } from "react-router-dom";
import { Sidebar } from "@/components/layout/sidebar";
import { useSignalR } from "@/hooks/use-signalr";
import { cn } from "@/lib/utils";

export default function ChatLayout() {
  useSignalR();
  const { chatId } = useParams<{ chatId: string }>();
  const hasChatOpen = !!chatId;

  return (
    <div className="flex h-screen overflow-hidden bg-background">
      <div
        className={cn(
          "md:flex md:w-72 md:shrink-0",
          hasChatOpen ? "hidden" : "flex w-full",
        )}
      >
        <Sidebar />
      </div>
      <main
        className={cn(
          "flex flex-col overflow-hidden",
          hasChatOpen ? "flex flex-1" : "hidden md:flex md:flex-1",
        )}
      >
        <Outlet />
      </main>
    </div>
  );
}
