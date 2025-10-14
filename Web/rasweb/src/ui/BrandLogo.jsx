export default function BrandLogo({ className = "h-12" }) {
    return (
        <div className="flex flex-col items-center gap-2">
            <img src="/assets/brand/ras-logo.svg" className={className} alt="RAS" />
            {/* optional tagline in brand script */}
        </div>
    );
}
