import { cn } from "@/lib/utils";
import { SentimentLabel } from "@/api/types";

interface SentimentDotProps {
  label: (typeof SentimentLabel)[keyof typeof SentimentLabel] | null;
  className?: string;
}

const labelMap: Record<
  (typeof SentimentLabel)[keyof typeof SentimentLabel],
  string
> = {
  [SentimentLabel.Positive]: "bg-emerald-400",
  [SentimentLabel.Neutral]: "bg-zinc-400",
  [SentimentLabel.Negative]: "bg-rose-400",
};

const titleMap: Record<
  (typeof SentimentLabel)[keyof typeof SentimentLabel],
  string
> = {
  [SentimentLabel.Positive]: "Positive sentiment",
  [SentimentLabel.Neutral]: "Neutral sentiment",
  [SentimentLabel.Negative]: "Negative sentiment",
};

export function SentimentDot({ label, className }: SentimentDotProps) {
  if (label === null) return null;

  return (
    <span
      className={cn(
        "inline-block h-2 w-2 rounded-full shrink-0",
        labelMap[label],
        className,
      )}
      title={titleMap[label]}
      aria-label={titleMap[label]}
    />
  );
}
