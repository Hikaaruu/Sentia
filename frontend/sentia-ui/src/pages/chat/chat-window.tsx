import { useParams, Navigate } from "react-router-dom";
import { useChats } from "@/hooks/use-chats";
import { useSendMessage } from "@/hooks/use-messages";
import { useSignalR } from "@/hooks/use-signalr";
import { ChatHeader } from "@/components/chat/chat-header";
import { MessageList } from "@/components/chat/message-list";
import { MessageInput } from "@/components/chat/message-input";

export default function ChatWindow() {
  const { chatId: chatIdParam } = useParams<{ chatId: string }>();
  const chatId = Number(chatIdParam);

  const { data: chats } = useChats();
  const { mutate: sendMessage } = useSendMessage(chatId);
  const { sendTyping } = useSignalR();

  const chat = chats?.find((c) => c.chatId === chatId);

  if (!chat) {
    // Still loading or invalid chatId
    return <Navigate to="/chats" replace />;
  }

  function handleSend(content: string) {
    sendMessage({ content });
  }

  function handleTyping() {
    sendTyping(chatId);
  }

  return (
    <div className="flex flex-1 flex-col overflow-hidden">
      <ChatHeader chat={chat} />
      <MessageList chatId={chatId} />
      <MessageInput onSend={handleSend} onTyping={handleTyping} />
    </div>
  );
}
