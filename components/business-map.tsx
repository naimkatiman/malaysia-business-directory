"use client"

import { useEffect, useRef, useState } from "react"
import { Loader } from "@googlemaps/js-api-loader"
import { Card, CardContent } from "@/components/ui/card"
import { MapPin } from "lucide-react"

interface Business {
  id: string
  name: string
  location: any
  is_premium: boolean
  categories: {
    name: string
  }
  rating: number
  review_count: number
}

interface BusinessMapProps {
  businesses: Business[]
  userLocation: { lat: number; lng: number } | null
}

export default function BusinessMap({ businesses, userLocation }: BusinessMapProps) {
  const mapRef = useRef<HTMLDivElement>(null)
  const [map, setMap] = useState<google.maps.Map | null>(null)
  const [markers, setMarkers] = useState<google.maps.Marker[]>([])
  const [infoWindow, setInfoWindow] = useState<google.maps.InfoWindow | null>(null)

  useEffect(() => {
    // Load Google Maps API
    const loader = new Loader({
      apiKey: process.env.NEXT_PUBLIC_GOOGLE_MAPS_API_KEY || "",
      version: "weekly",
      libraries: ["places"],
    })

    loader.load().then((google) => {
      if (mapRef.current) {
        // Default to Kuala Lumpur if no user location
        const defaultLocation = { lat: 3.139, lng: 101.6869 }
        const mapCenter = userLocation || defaultLocation

        const mapInstance = new google.maps.Map(mapRef.current, {
          center: mapCenter,
          zoom: 12,
          styles: [
            {
              featureType: "poi",
              elementType: "labels",
              stylers: [{ visibility: "off" }],
            },
          ],
        })

        setMap(mapInstance)
        setInfoWindow(new google.maps.InfoWindow())
      }
    })
  }, [userLocation])

  // Update markers when businesses or map changes
  useEffect(() => {
    if (!map || !infoWindow) return

    // Clear existing markers
    markers.forEach((marker) => marker.setMap(null))
    const newMarkers: google.maps.Marker[] = []

    // Add user location marker if available
    if (userLocation) {
      const userMarker = new google.maps.Marker({
        position: userLocation,
        map,
        icon: {
          path: google.maps.SymbolPath.CIRCLE,
          scale: 10,
          fillColor: "#4285F4",
          fillOpacity: 1,
          strokeColor: "#ffffff",
          strokeWeight: 2,
        },
        title: "Your Location",
      })
      newMarkers.push(userMarker)
    }

    // Add business markers
    businesses.forEach((business) => {
      try {
        // Extract coordinates from PostGIS point
        const coords = business.location.coordinates || [0, 0]
        const position = { lat: coords[1], lng: coords[0] }

        const marker = new google.maps.Marker({
          position,
          map,
          title: business.name,
          icon: business.is_premium
            ? {
                path: google.maps.SymbolPath.BACKWARD_CLOSED_ARROW,
                scale: 6,
                fillColor: "#f59e0b",
                fillOpacity: 1,
                strokeColor: "#ffffff",
                strokeWeight: 2,
              }
            : undefined,
        })

        // Create info window content
        const content = `
          <div class="p-2 max-w-xs">
            <h3 class="font-semibold">${business.name}</h3>
            <div class="text-sm text-gray-600">${business.categories?.name || ""}</div>
            <div class="flex items-center mt-1">
              <span class="text-amber-500">★</span>
              <span class="ml-1 text-sm">${business.rating.toFixed(1)}</span>
              <span class="text-xs text-gray-500 ml-1">(${business.review_count} reviews)</span>
            </div>
            <a href="/businesses/${business.id}" class="text-sm text-blue-600 hover:underline mt-2 inline-block">View Details</a>
          </div>
        `

        marker.addListener("click", () => {
          infoWindow.setContent(content)
          infoWindow.open(map, marker)
        })

        newMarkers.push(marker)
      } catch (error) {
        console.error("Error creating marker for business:", business.id, error)
      }
    })

    setMarkers(newMarkers)

    // Fit bounds to show all markers if we have businesses
    if (businesses.length > 0) {
      const bounds = new google.maps.LatLngBounds()
      newMarkers.forEach((marker) => {
        bounds.extend(marker.getPosition()!)
      })
      map.fitBounds(bounds)

      // Don't zoom in too far
      const listener = google.maps.event.addListener(map, "idle", () => {
        if (map.getZoom()! > 16) map.setZoom(16)
        google.maps.event.removeListener(listener)
      })
    }

    return () => {
      newMarkers.forEach((marker) => marker.setMap(null))
    }
  }, [map, businesses, userLocation, infoWindow])

  return (
    <div ref={mapRef} className="w-full h-full">
      {!process.env.NEXT_PUBLIC_GOOGLE_MAPS_API_KEY && (
        <div className="absolute inset-0 flex items-center justify-center bg-muted/50 z-10">
          <Card className="w-96">
            <CardContent className="p-4">
              <div className="flex flex-col items-center text-center gap-2">
                <MapPin className="h-8 w-8 text-muted-foreground" />
                <h3 className="font-semibold">Google Maps API Key Required</h3>
                <p className="text-sm text-muted-foreground">
                  To display the map, add your Google Maps API key to the environment variables.
                </p>
              </div>
            </CardContent>
          </Card>
        </div>
      )}
    </div>
  )
}

