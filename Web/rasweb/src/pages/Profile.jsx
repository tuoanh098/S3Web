// src/pages/Profile.jsx
import React from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";
import Avatar from "../ui/Avatar";

// small helper to read from localStorage safely
function readLocalJson(key) {
    try {
        const raw = localStorage.getItem(key);
        if (!raw) return null;
        return JSON.parse(raw);
    } catch {
        return null;
    }
}

function normalizeProfile(raw) {
    if (!raw) return null;
    return {
        userId: raw.user_id ?? raw.userId ?? raw.user_id,
        firstName: raw.first_name ?? raw.firstName ?? raw.first,
        lastName: raw.last_name ?? raw.lastName ?? raw.last,
        birthday: raw.birthday ?? raw.birthdate ?? raw.birthday,
        gender: raw.gender ?? raw.sex,
        nation: raw.nation ?? raw.country,
        email: raw.email ?? raw.emailAddress,
        mobile: raw.mobile ?? raw.phone ?? raw.phoneNumber,
        parent: raw.parent ?? raw.parents,
        bio: raw.bio ?? raw.about ?? raw.description,
        joinedAt: raw.joined_at ?? raw.joinedAt ?? raw.createdAt
    };
}

// Simple info display component
const InfoField = ({ label, value, placeholder }) => (
    <div>
        <label className="text-sm font-medium text-gray-500">{label}</label>
        <p className="mt-1 text-base text-gray-900">
            {value || <span className="text-gray-400">{placeholder}</span>}
        </p>
    </div>
);

export default function Profile() {
    const nav = useNavigate();
    const auth = useAuth(); // may provide identity/profile depending on your AuthContext
    const identityFromCtx = auth?.identity ?? null;
    const profileFromCtx = auth?.profile ?? null;

    // fallback to localStorage if context didn't provide
    const identityStored = readLocalJson("identity");
    const profileStored = readLocalJson("profile");

    const identity = identityFromCtx || identityStored || null;
    const rawProfile = profileFromCtx || profileStored || null;
    const profile = normalizeProfile(rawProfile);

    if (!identity && !profile) {
        return <div className="text-center p-10">Loading profile...</div>;
    }

    // displayName: prefer lastName + firstName (payload uses last_name and first_name)
    const displayName = profile
        ? `${profile.lastName || ""} ${profile.firstName || ""}`.trim().toUpperCase()
        : (identity?.username || identity?.email || "User").toUpperCase();

    const formattedBirthday = profile?.birthday
        ? new Date(profile.birthday).toLocaleDateString("vi-VN")
        : null;

    return (
        <div className="bg-gray-100 min-h-screen">
            <div className="max-w-5xl mx-auto py-8 px-4">
                {/* Header Profile */}
                <div className="bg-white rounded-lg shadow-md overflow-hidden">
                    <div
                        className="h-40 bg-blue-800 bg-opacity-75 relative"
                        style={{ backgroundImage: "url('https://res.cloudinary.com/ras-vi/image/upload/v1655389656/ras-upload/trang-chu/street-wall-bg_pyqtlz.jpg')" }}
                    >
                        <div className="absolute -bottom-12 left-1/2 -translate-x-1/2">
                            <Avatar name={displayName} size={96} />
                        </div>
                    </div>
                    <div className="pt-16 pb-4 text-center">
                        <h1 className="text-3xl font-bold text-gray-800">{displayName}</h1>
                        <p className="text-sm text-gray-500 mt-1">{identity?.email ?? profile?.email}</p>
                    </div>
                </div>

                {/* Body Profile */}
                <div className="mt-8 grid grid-cols-1 md:grid-cols-3 gap-8">
                    {/* Left Column */}
                    <div className="md:col-span-1 space-y-8">
                        <div className="bg-white p-6 rounded-lg shadow-md">
                            <h2 className="text-xl font-semibold mb-4">Contact</h2>
                            <div className="space-y-4">
                                <InfoField label="Email" value={identity?.email ?? profile?.email} />
                                <InfoField label="Mobile" value={profile?.mobile} />
                                <InfoField label="Preferred contact" value="Email" placeholder="Not set" />
                            </div>
                        </div>
                        <div className="bg-white p-6 rounded-lg shadow-md">
                            <h2 className="text-xl font-semibold mb-4">Basic Information</h2>
                            <div className="space-y-4">
                                <InfoField label="Birthday" value={formattedBirthday} />
                                <InfoField label="Gender" value={profile?.gender} placeholder="Prefer not to say" />
                                <InfoField label="Nationality" value={profile?.nation} placeholder="Not set" />
                            </div>
                        </div>
                    </div>

                    {/* Right Column */}
                    <div className="md:col-span-2 space-y-8">
                        <div className="bg-white p-6 rounded-lg shadow-md">
                            <div className="flex justify-between items-center mb-4">
                                <h2 className="text-xl font-semibold">My Story</h2>
                                <button
                                    onClick={() => nav("/profile/edit")}
                                    className="bg-blue-600 text-white px-4 py-2 rounded-lg text-sm hover:bg-blue-700"
                                >
                                    Edit
                                </button>
                            </div>
                            <div className="space-y-4">
                                <InfoField label="About me" value={profile?.bio} placeholder="Tell us about yourself..." />
                                <InfoField label="Parent" value={profile?.parent} placeholder="Not set" />
                                <InfoField label="Joined" value={profile?.joinedAt ? new Date(profile.joinedAt).toLocaleString("vi-VN") : null} placeholder="Date unknown" />
                            </div>
                        </div>

                        <div className="bg-white p-6 rounded-lg shadow-md">
                            <h2 className="text-xl font-semibold mb-4">Personalization</h2>
                            <div className="space-y-4">
                                <InfoField label="Personal Motivation" value="Personal Development" placeholder="Not set" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}
