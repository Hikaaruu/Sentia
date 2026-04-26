import { Outlet } from "react-router-dom";
import { Sidebar } from "@/components/layout/sidebar";
import { useSignalR } from "@/hooks/use-signalr";

export default function ChatLayout() {
  useSignalR();

  return (
    <div className="flex h-screen overflow-hidden bg-background">
      <Sidebar />
      <main className="flex flex-1 flex-col overflow-hidden">
        <Outlet />
      </main>
    </div>
  );
}
