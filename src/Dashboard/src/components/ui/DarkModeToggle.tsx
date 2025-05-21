import { Moon, Sun } from "lucide-react"
import { Button } from "@/components/ui/button"
import { useEffect, useState } from "react"

export function DarkModeToggle() {
    const [isDark, setIsDark] = useState<boolean>(() => {
        // Try to read from localStorage on load
        if (typeof window !== "undefined") {
            const stored = localStorage.getItem("theme")
            return stored === "dark" || (
                stored === null && window.matchMedia("(prefers-color-scheme: dark)").matches
            )
        }
        return false
    })

    useEffect(() => {
        // Update class on <html>
        document.documentElement.classList.toggle("dark", isDark)
        // Save preference to localStorage
        localStorage.setItem("theme", isDark ? "dark" : "light")
    }, [isDark])

    return (
        <Button
            variant="outline"
            size="icon"
            onClick={() => setIsDark(prev => !prev)}
            className="rounded-full"
            aria-label="Toggle theme"
        >
            {isDark ? <Sun className="h-4 w-4" /> : <Moon className="h-4 w-4" />}
        </Button>
    )
}