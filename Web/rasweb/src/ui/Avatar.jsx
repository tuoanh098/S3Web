import React from "react";

export default function Avatar({ name, size = 96, className = "" }) {
    const initials = (name || "U").split(" ").map(s => s[0]).slice(0, 2).join("").toUpperCase();
    const s = `${size}px`;
    return (
        <div
            className={`inline-flex items-center justify-center rounded-full bg-ras-indigo text-white font-bold ${className}`}
            style={{ width: s, height: s, lineHeight: s, fontSize: Math.max(14, size / 3) }}
            aria-hidden
        >
            {initials}
        </div>
    );
}
