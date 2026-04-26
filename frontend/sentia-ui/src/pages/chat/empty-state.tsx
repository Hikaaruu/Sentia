import { MessageSquareDashed } from "lucide-react";

export default function EmptyState() {
  return (
    <div className="flex flex-1 flex-col items-center justify-center gap-3 text-center text-muted-foreground">
      <MessageSquareDashed className="h-12 w-12 opacity-30" />
      <div className="space-y-1">
        <p className="text-sm font-medium">No conversation selected</p>
        <p className="text-xs opacity-70">
          Choose a chat from the sidebar or start a new one.
        </p>
      </div>
    </div>
  );
}
