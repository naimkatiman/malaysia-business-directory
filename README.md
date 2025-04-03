# Malaysia Business Directory

A comprehensive web application for browsing, searching, and managing business listings across Malaysia.

![Malaysia Business Directory](https://github.com/naimkatiman/malaysia-business-directory/raw/main/public/placeholder.jpg)

## Features

- **Business Listings**: Browse businesses by category, location, and popularity
- **Search Functionality**: Find businesses using keywords, categories, or locations
- **Business Profiles**: View detailed business information, including contact details, operating hours, and reviews
- **User Authentication**: Sign up, sign in, and manage your profile
- **Business Management**: Claim and manage your business listings
- **Premium Listings**: Highlight your business with premium features
- **Maps Integration**: View businesses on an interactive map
- **Mobile Responsive**: Optimized for all device sizes

## Tech Stack

- **Frontend**: Next.js 14, React, TypeScript
- **UI**: Tailwind CSS, shadcn/ui component library
- **Backend**: Supabase (PostgreSQL with PostGIS extension)
- **Authentication**: Supabase Auth
- **Storage**: Supabase Storage
- **Maps**: Integration-ready for map services
- **Deployment**: Vercel-compatible

## Getting Started

### Prerequisites

- Node.js 18+ and pnpm installed
- Supabase account and project

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/naimkatiman/malaysia-business-directory.git
   cd malaysia-business-directory
   ```

2. Install dependencies:
   ```bash
   pnpm install
   ```

3. Set up environment variables:
   Create a `.env.local` file in the root directory with the following variables:
   ```
   NEXT_PUBLIC_SUPABASE_URL=your_supabase_url
   NEXT_PUBLIC_SUPABASE_ANON_KEY=your_supabase_anon_key
   ```

4. Run the development server:
   ```bash
   pnpm dev
   ```

5. Open [http://localhost:3000](http://localhost:3000) in your browser to see the application.

## Project Structure

- `/app`: Next.js 14 app directory with pages and API routes
- `/components`: React components including UI elements and business-specific components
- `/lib`: Utility functions and Supabase client
- `/public`: Static assets
- `/styles`: Global CSS and Tailwind configurations
- `/types`: TypeScript type definitions for the database and components

## Database Schema

The application is built around these main tables:
- `businesses`: Core business listings with location data using PostGIS
- `categories`: Business categories
- `reviews`: User reviews for businesses
- `users`: User profiles with authentication

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Contact

For any inquiries about this project, please open an issue on the GitHub repository.
