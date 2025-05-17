import { useEffect, useState } from "react";
import { Card, CardContent } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Textarea } from "@/components/ui/textarea";

type Review = {
    reviewId: string;
    author: string;
    comment: string;
    sentiment: string;
    suggestedResponse: string;
    status: string;
};

export function ReviewList() {
    const [reviews, setReviews] = useState<Review[]>([]);
    const [loading, setLoading] = useState(true);
    const [drafts, setDrafts] = useState<Record<string, string>>({});

    // 🔄 Fetch reviews from API
    useEffect(() => {
        async function fetchReviews() {
            try {
                const res = await fetch("https://localhost:7157/api/reviews");
                const json = await res.json();
                setReviews(json.data ?? []);

                // Set initial drafts to match suggested responses
                const initialDrafts = Object.fromEntries(
                    (json.data ?? []).map((r: Review) => [r.reviewId, r.suggestedResponse])
                );
                setDrafts(initialDrafts);
            } catch (err) {
                console.error("Failed to fetch reviews", err);
            } finally {
                setLoading(false);
            }
        }

        fetchReviews();
    }, []);

    // ✅ Approve review with current draft
    const handleApprove = async (reviewId: string) => {
        const finalResponse = drafts[reviewId];

        try {
            const res = await fetch("http://localhost:7157/api/reviews/respond", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({ reviewId, finalResponse }),
            });

            if (res.ok) {
                // Mark review as responded
                setReviews((prev) =>
                    prev.map((r) =>
                        r.reviewId === reviewId ? { ...r, status: "Responded" } : r
                    )
                );
            } else {
                console.error("❌ Failed to approve review");
            }
        } catch (err) {
            console.error("❌ Error sending approval", err);
        }
    };

    if (loading)
        return (
            <p className="text-muted-foreground px-4">Loading reviews...</p>
        );

    return (
        <div className="grid gap-4 px-4">
            {reviews.map((r) => (
                <Card key={r.reviewId}>
                    <CardContent className="pt-4 space-y-3">
                        {/* Header with author and sentiment */}
                        <div className="flex justify-between items-center">
                            <h3 className="font-semibold">{r.author}</h3>
                            <Badge
                                variant={
                                    r.sentiment === "Positive"
                                        ? "default"
                                        : r.sentiment === "Negative"
                                            ? "destructive"
                                            : "secondary"
                                }
                            >
                                {r.sentiment}
                            </Badge>
                        </div>

                        {/* Original comment */}
                        <p className="text-sm text-muted-foreground">{r.comment}</p>

                        {/* Editable response */}
                        <Textarea
                            value={drafts[r.reviewId] ?? ""}
                            onChange={(e: React.ChangeEvent<HTMLTextAreaElement>) =>
                                setDrafts((prev) => ({
                                    ...prev,
                                    [r.reviewId]: e.target.value,
                                }))
                            }
                            className="text-sm"
                            placeholder="Edit the suggested response..."
                        />

                        {/* Status and Approve button */}
                        <div className="flex justify-between items-center">
                            <p className="text-xs text-muted-foreground">Status: {r.status}</p>
                            {r.status !== "Responded" && (
                                <Button
                                    size="sm"
                                    onClick={() => handleApprove(r.reviewId)}
                                >
                                    Approve
                                </Button>
                            )}
                        </div>
                    </CardContent>
                </Card>
            ))}
        </div>
    );
}