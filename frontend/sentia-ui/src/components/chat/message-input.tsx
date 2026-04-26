import { useRef, type KeyboardEvent } from "react";
import { useSignalRStore } from "@/stores/signalr.store";
import { cn } from "@/lib/utils";

interface MessageInputProps {
  onSend: (content: string) => void;
  onTyping: () => void;
}

export function MessageInput({ onSend, onTyping }: MessageInputProps) {
  const status = useSignalRStore((s) => s.status);
  const ref = useRef<HTMLTextAreaElement>(null);

  function handleKeyDown(e: KeyboardEvent<HTMLTextAreaElement>) {
    if (e.key === "Enter" && !e.shiftKey) {
      e.preventDefault();
      submit();
    }
  }

  function submit() {
    const content = ref.current?.value.trim();
    if (!content) return;
    onSend(content);
    if (ref.current) ref.current.value = "";
    ref.current?.focus();
  }

  const isDisabled = status !== "connected";

  return (
    <div className="border-t border-border bg-background px-4 py-3">
      <div
        className={cn(
          "flex items-end gap-2 rounded-xl border border-border bg-muted/40 px-3 py-2 transition-colors",
          isDisabled && "opacity-50",
        )}
      >
        <textarea
          ref={ref}
          rows={1}
          disabled={isDisabled}
          placeholder={isDisabled ? "Connecting…" : "Message…"}
          className="max-h-32 flex-1 resize-none bg-transparent text-sm outline-none placeholder:text-muted-foreground disabled:cursor-not-allowed"
          onKeyDown={handleKeyDown}
          onChange={onTyping}
          style={{ overflowY: "auto" }}
        />
        <button
          type="button"
          disabled={isDisabled}
          onClick={submit}
          className="mb-0.5 shrink-0 rounded-lg bg-primary px-3 py-1.5 text-xs font-medium text-primary-foreground transition-opacity hover:opacity-90 disabled:cursor-not-allowed disabled:opacity-50"
        >
          Send
        </button>
      </div>
    </div>
  );
}
