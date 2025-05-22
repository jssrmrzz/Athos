import { useApi } from "@/hooks/useApi"
import { useMockApiContext } from "@/context/MockApiContext"
import { Button } from "@/components/ui/button"
import { useToast } from "@/components/ui/use-toast" // Toast hook

export function DebugPanel({ onRefresh }: { onRefresh: () => void }) {
    const api = useApi()
    const { useMockApi } = useMockApiContext()
    const { toast } = useToast() // ✅ Access toast function

    const handleReset = async () => {
        try {
            await api.resetMockData()
            await onRefresh()

            toast(
                <div>
                    <p className="font-semibold">Mock Data Reset</p>
                    <p className="text-sm text-muted-foreground">All reviews have been restored to default.</p>
                </div>
            )
        } catch (err) {
            toast(
                <div>
                    <p className="font-semibold text-destructive">Reset Failed</p>
                    <p className="text-sm text-muted-foreground">Something went wrong while resetting mock data.</p>
                </div>
            )
            console.error("❌ Failed to reset mock data", err)
        }
    }

    if (!useMockApi) return null

    return (
        <div className="fixed bottom-4 left-4 z-50 bg-muted p-3 rounded-lg shadow-lg space-y-2">
            <p className="text-xs text-muted-foreground font-semibold">🧪 Sandbox Tools</p>
            <Button variant="secondary" size="sm" onClick={handleReset}>
                ♻️ Reset Mock Data
            </Button>
        </div>
    )
}