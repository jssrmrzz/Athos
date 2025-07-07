import { useMockApiContext } from "@/context/MockApiContext"

export function useApi() {
    const { useMockApi } = useMockApiContext()

    // 👇 Fallback to your dev machine IP for mobile devices
    const isLocalhost = window.location.hostname === "localhost"
    const fallbackIp = "10.0.0.22" // Replace this with your machine’s IP if it changes
    const host = isLocalhost ? "localhost" : fallbackIp

    const baseUrl = useMockApi
        ? `https://${host}:7157/api/mock`
        : `https://${host}:7157/api`

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

    const suggestResponse = async ({
                                       reviewId,
                                       author,
                                       comment,
                                   }: {
        reviewId: string
        author: string
        comment: string
    }) => {
        const res = await fetch(`${baseUrl}/llm/suggest`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ reviewId, author, comment }),
        })

        if (!res.ok) throw new Error("Suggestion failed")

        const json = await res.json()
        return json.suggestion
    }

    const resetMockData = async () => {
        if (!useMockApi) return
        const res = await fetch(`${baseUrl}/reset`, { method: "POST" })
        if (!res.ok) throw new Error("Failed to reset mock data")
    }

    return {
        getReviews,
        postResponse,
        resetMockData,
        suggestResponse,
        baseUrl,
    }
}