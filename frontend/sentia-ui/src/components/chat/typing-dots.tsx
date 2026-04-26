export function TypingDots() {
  return (
    <span className="inline-flex items-center gap-[2px] mt-1 ml-0.5">
      <span
        className="animate-bounce-dot inline-block h-1 w-1 rounded-full bg-current"
        style={{ animationDelay: "0ms" }}
      />
      <span
        className="animate-bounce-dot inline-block h-1 w-1 rounded-full bg-current"
        style={{ animationDelay: "150ms" }}
      />
      <span
        className="animate-bounce-dot inline-block h-1 w-1 rounded-full bg-current"
        style={{ animationDelay: "300ms" }}
      />
    </span>
  );
}
