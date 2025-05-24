import { useMockApiContext } from "@/context/MockApiContext"

export function useApi() {
    const { useMockApi } = useMockApiContext()

    const baseUrl = useMockApi
        ? "https://localhost:7157/api/mock"
        : "https://localhost:7157/api"

    const getReviews = async (page = 1, pageSize = 10) => {
        const res = await fetch(`${baseUrl}/reviews?page=${page}&pageSize=${pageSize}`)
        const json = await res.json()
        return {
            data: json.data ?? [],
            total: json.total ?? 0
        }
    }

    const postResponse = async (reviewId: string, finalResponse: string) => {
        const res = await fetch(`${baseUrl}/respond`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ reviewId, finalResponse }),
        })

        if (!res.ok) throw new Error("Failed to submit response")
    }

    // Reset endpoint
    const resetMockData = async () => {
        if (!useMockApi) return // Only valid in mock mode
        const res = await fetch(`${baseUrl}/reset`, { method: "POST" })
        if (!res.ok) throw new Error("Failed to reset mock data")
    }

    return {
        getReviews,
        postResponse,
        resetMockData,
        baseUrl
    }
}