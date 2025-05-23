type EmptyStateProps = {
    message: string
    actionText?: string
    onAction?: () => void
}

export function EmptyState({ message, actionText, onAction }: EmptyStateProps) {
    return (
        <div className="text-center py-10 text-muted-foreground space-y-2">
            <p className="text-sm">{message}</p>
            {actionText && onAction && (
                <button
                    onClick={onAction}
                    className="mt-2 text-sm text-primary underline underline-offset-4"
                >
                    {actionText}
                </button>
            )}
        </div>
    )
}