import { cn } from "@/lib/utils";
import type { MessageDto } from "@/api/types";
import { SentimentDot } from "./sentiment-dot";
import { ReadReceipt } from "./read-receipt";

interface MessageBubbleProps {
  message: MessageDto;
  isOwn: boolean;
  isRead: boolean;
  isOptimistic?: boolean;
}

function formatTime(iso: string) {
  return new Date(iso).toLocaleTimeString([], {
    hour: "2-digit",
    minute: "2-digit",
  });
}

export function MessageBubble({
  message,
  isOwn,
  isRead,
  isOptimistic = false,
}: MessageBubbleProps) {
  return (
    <div className={cn("flex", isOwn ? "justify-end" : "justify-start")}>
      <div
        className={cn(
          "relative max-w-[70%] rounded-2xl px-3.5 py-2 text-sm shadow-xs",
          isOwn
            ? "rounded-br-sm bg-primary text-primary-foreground"
            : "rounded-bl-sm bg-muted text-foreground",
          isOptimistic && "opacity-60",
        )}
      >
        <p className="whitespace-pre-wrap break-words leading-relaxed">
          {message.content}
        </p>

        <div
          className={cn(
            "mt-1 flex items-center gap-1.5",
            isOwn ? "justify-end" : "justify-start",
          )}
        >
          <SentimentDot label={message.sentimentLabel} />
          <span
            className={cn(
              "text-[10px] leading-none",
              isOwn ? "text-primary-foreground/70" : "text-muted-foreground",
            )}
          >
            {formatTime(message.createdAt)}
          </span>
          {isOwn && <ReadReceipt isRead={isRead} />}
        </div>
      </div>
    </div>
  );
}
