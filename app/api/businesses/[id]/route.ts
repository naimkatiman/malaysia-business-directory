import { NextResponse } from "next/server"
import { createServerSupabaseClient } from "@/lib/supabase"

export async function GET(request: Request, { params }: { params: { id: string } }) {
  const id = params.id
  const supabase = createServerSupabaseClient()

  try {
    // First try to find by id (which could be a UUID or a slug)
    let query = supabase.from("businesses").select(`
        *,
        categories(*),
        business_images(*),
        reviews(*, users(name)),
        tags:business_tags(tags(*))
      `)

    // Check if id is a UUID
    const isUuid = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i.test(id)

    if (isUuid) {
      query = query.eq("id", id)
    } else {
      // Use ilike for case-insensitive matching on name
      // Convert name to slug format for comparison
      query = query.ilike("name", id.replace(/-/g, "%"))
    }

    const { data, error } = await query.single()

    if (error) {
      throw error
    }

    if (!data) {
      return NextResponse.json({ error: "Business not found" }, { status: 404 })
    }

    // Log view for analytics
    const { error: viewError } = await supabase.from("business_views").insert([
      {
        business_id: data.id,
        // user_id would be added here if authenticated
      },
    ])

    if (viewError) {
      console.error("Error logging view:", viewError)
    }

    return NextResponse.json({ business: data })
  } catch (error) {
    console.error("Error fetching business:", error)
    return NextResponse.json({ error: "Failed to fetch business" }, { status: 500 })
  }
}

