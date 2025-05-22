import { useMockApiContext } from "@/context/MockApiContext"

export function useApi() {
    const { useMockApi } = useMockApiContext()

    const baseUrl = useMockApi
        ? "https://localhost:7157/api/mock"
        : "https://localhost:7157/api"

    const getReviews = async () => {
        const res = await fetch(`${baseUrl}/reviews`)
        if (!res.ok) throw new Error("Failed to fetch reviews")
        const json = await res.json()
        return json.data ?? []
    }

    const postResponse = async (reviewId: string, finalResponse: string) => {
        const res = await fetch(`${baseUrl}/respond`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ reviewId, finalResponse }),
        })

        if (!res.ok) throw new Error("Failed to submit response")
    }

    return {
        getReviews,
        postResponse,
        baseUrl // Optional: useful for debugging or logging
    }
}