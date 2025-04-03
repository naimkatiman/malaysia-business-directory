import { NextResponse } from "next/server"
import { createServerSupabaseClient } from "@/lib/supabase"

export async function GET(request: Request) {
  const { searchParams } = new URL(request.url)
  const query = searchParams.get("query") || ""
  const categoryId = searchParams.get("category") || ""
  const tagIds = searchParams.getAll("tags") || []
  const lat = searchParams.get("lat") ? Number.parseFloat(searchParams.get("lat")!) : null
  const lng = searchParams.get("lng") ? Number.parseFloat(searchParams.get("lng")!) : null
  const radius = searchParams.get("radius") ? Number.parseInt(searchParams.get("radius")!) : 5000 // Default 5km
  const page = Number.parseInt(searchParams.get("page") || "1")
  const limit = Number.parseInt(searchParams.get("limit") || "10")
  const offset = (page - 1) * limit

  const supabase = createServerSupabaseClient()

  try {
    let query_builder = supabase
      .from("businesses")
      .select(`
        *,
        categories(*),
        business_images(id, url, is_primary),
        tags:business_tags(tag_id)
      `)
      .order("is_premium", { ascending: false })
      .order("rating", { ascending: false })
      .range(offset, offset + limit - 1)

    // Apply text search if provided
    if (query) {
      query_builder = query_builder.ilike("name", `%${query}%`)
    }

    // Filter by category if provided
    if (categoryId) {
      query_builder = query_builder.eq("category_id", categoryId)
    }

    // Filter by tags if provided
    if (tagIds.length > 0) {
      query_builder = query_builder.or(tagIds.map((tagId) => `tags.tag_id.eq.${tagId}`).join(","))
    }

    // Apply geospatial query if coordinates are provided
    if (lat !== null && lng !== null) {
      // Use the stored procedure we created
      query_builder = query_builder.rpc("businesses_within_distance", {
        lat: lat,
        lng: lng,
        distance_meters: radius,
      })
    }

    const { data: businesses, error, count } = await query_builder

    if (error) {
      throw error
    }

    // Get total count for pagination
    const { count: totalCount, error: countError } = await supabase.from("businesses").count()

    if (countError) {
      throw countError
    }

    return NextResponse.json({
      businesses,
      pagination: {
        total: totalCount,
        page,
        limit,
        pages: Math.ceil(Number.parseInt(totalCount) / limit),
      },
    })
  } catch (error) {
    console.error("Error searching businesses:", error)
    return NextResponse.json({ error: "Failed to search businesses" }, { status: 500 })
  }
}

