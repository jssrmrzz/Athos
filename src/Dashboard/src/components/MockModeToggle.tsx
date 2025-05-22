import { Switch } from "@/components/ui/switch"
import { Label } from "@/components/ui/label"
import { useMockApiContext } from "@/context/MockApiContext"

export function MockModeToggle() {
    const { useMockApi, toggleMockApi } = useMockApiContext()

    return (
        <div className="flex items-center space-x-3 p-2">
            <Switch
                id="mock-mode-switch"
                checked={useMockApi}
                onCheckedChange={toggleMockApi}
            />
            <Label htmlFor="mock-mode-switch">Mock Mode</Label>
        </div>
    )
}