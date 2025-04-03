import { NextResponse } from "next/server"
import { createServerSupabaseClient } from "@/lib/supabase"

export async function GET(request: Request) {
  const { searchParams } = new URL(request.url)
  const query = searchParams.get("query") || ""
  const category = searchParams.get("category") || ""
  const lat = searchParams.get("lat") || null
  const lng = searchParams.get("lng") || null
  const radius = searchParams.get("radius") || "10000" // Default 10km in meters

  const supabase = createServerSupabaseClient()

  try {
    let businessQuery = supabase.from("businesses").select(`
        *,
        categories(*)
      `)

    // Apply filters
    if (query) {
      businessQuery = businessQuery.ilike("name", `%${query}%`)
    }

    if (category) {
      businessQuery = businessQuery.eq("category_id", category)
    }

    // Apply geospatial query if coordinates are provided
    if (lat && lng) {
      // Using PostGIS ST_DWithin to find businesses within radius
      businessQuery = businessQuery.rpc("businesses_within_distance", {
        lat: Number.parseFloat(lat),
        lng: Number.parseFloat(lng),
        distance_meters: Number.parseInt(radius),
      })
    }

    const { data, error } = await businessQuery

    if (error) {
      throw error
    }

    return NextResponse.json({ businesses: data })
  } catch (error) {
    console.error("Error fetching businesses:", error)
    return NextResponse.json({ error: "Failed to fetch businesses" }, { status: 500 })
  }
}

