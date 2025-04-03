import { NextResponse } from "next/server"
import { createServerSupabaseClient } from "@/lib/supabase"

export async function GET() {
  const supabase = createServerSupabaseClient()

  try {
    const { data, error } = await supabase.from("categories").select("*").order("name")

    if (error) {
      throw error
    }

    return NextResponse.json({ categories: data })
  } catch (error) {
    console.error("Error fetching categories:", error)
    return NextResponse.json({ error: "Failed to fetch categories" }, { status: 500 })
  }
}

