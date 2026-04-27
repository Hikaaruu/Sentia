import { useParams, Navigate } from "react-router-dom";
import { useChats } from "@/hooks/use-chats";
import { useSendMessage } from "@/hooks/use-messages";
import { sendTypingIndicator } from "@/hooks/use-signalr";
import { ChatHeader } from "@/components/chat/chat-header";
import { MessageList } from "@/components/chat/message-list";
import { MessageInput } from "@/components/chat/message-input";
import { ulid } from "ulid";

export default function ChatWindow() {
  const { chatId: chatIdParam } = useParams<{ chatId: string }>();
  const chatId = Number(chatIdParam);

  const { data: chats, isLoading } = useChats();
  const { mutate: sendMessage } = useSendMessage(chatId);

  if (isLoading) {
    return (
      <div className="flex flex-1 items-center justify-center text-sm text-muted-foreground">
        Loading chat...
      </div>
    );
  }

  const chat = chats?.find((c) => c.chatId === chatId);

  if (!chat) {
    return <Navigate to="/chats" replace />;
  }

  function handleSend(content: string) {
    sendMessage({ messageId: ulid(), content });
  }

  function handleTyping() {
    sendTypingIndicator(chatId);
  }

  return (
    <div className="flex flex-1 flex-col overflow-hidden">
      <ChatHeader chat={chat} />
      <MessageList chatId={chatId} />
      <MessageInput onSend={handleSend} onTyping={handleTyping} />
    </div>
  );
}
