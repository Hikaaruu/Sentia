export function TypingDots() {
  return (
    <span
      className="flex items-center gap-0.5"
      aria-label="typing"
      role="status"
    >
      <span
        className="h-1.5 w-1.5 rounded-full bg-current animate-[bounce-dot_1.2s_ease-in-out_infinite]"
        style={{ animationDelay: "0ms" }}
      />
      <span
        className="h-1.5 w-1.5 rounded-full bg-current animate-[bounce-dot_1.2s_ease-in-out_infinite]"
        style={{ animationDelay: "150ms" }}
      />
      <span
        className="h-1.5 w-1.5 rounded-full bg-current animate-[bounce-dot_1.2s_ease-in-out_infinite]"
        style={{ animationDelay: "300ms" }}
      />
    </span>
  );
}
