import { NextResponse } from "next/server"
import { createServerSupabaseClient } from "@/lib/supabase"

export async function GET() {
  const supabase = createServerSupabaseClient()

  try {
    const { data, error } = await supabase.from("tags").select("*").order("name")

    if (error) {
      throw error
    }

    return NextResponse.json({ tags: data })
  } catch (error) {
    console.error("Error fetching tags:", error)
    return NextResponse.json({ error: "Failed to fetch tags" }, { status: 500 })
  }
}

