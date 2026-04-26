import { Check, CheckCheck } from "lucide-react";
import { cn } from "@/lib/utils";

interface ReadReceiptProps {
  isRead: boolean;
  className?: string;
}

export function ReadReceipt({ isRead, className }: ReadReceiptProps) {
  return isRead ? (
    <CheckCheck
      className={cn("h-3.5 w-3.5 text-sky-500 shrink-0", className)}
      aria-label="Read"
    />
  ) : (
    <Check
      className={cn("h-3.5 w-3.5 text-muted-foreground shrink-0", className)}
      aria-label="Sent"
    />
  );
}
