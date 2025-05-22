import { useMockApiContext } from "@/context/MockApiContext"

export function MockModeBanner() {
    const { useMockApi } = useMockApiContext()

    if (!useMockApi) return null

    return (
        <div className="bg-yellow-100 text-yellow-900 text-sm text-center py-2 shadow-md">
            🧪 Sandbox Mode Active — Data is mock only
        </div>
    )
}