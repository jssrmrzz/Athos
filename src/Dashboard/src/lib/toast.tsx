import { toast } from "sonner"

export const showSuccessToast = (title: string, description?: string) => {
    toast(
        <div>
            <p className="font-semibold">{title}</p>
    {description && (
        <p className="text-sm text-muted-foreground">{description}</p>
    )}
    </div>
)
}

export const showErrorToast = (title: string, description?: string) => {
    toast(
        <div>
            <p className="font-semibold text-destructive">{title}</p>
    {description && (
        <p className="text-sm text-muted-foreground">{description}</p>
    )}
    </div>
)
}