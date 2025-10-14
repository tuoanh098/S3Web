import React, { useState } from "react";
import { AuthApi } from "../api";

export default function EditProfile({ profile = {}, onSaved = () => { }, onCancel = () => { } }) {
    const [displayName, setDisplayName] = useState(profile.displayName || profile.name || "");
    const [bio, setBio] = useState(profile.bio || "");
    const [saving, setSaving] = useState(false);
    const [err, setErr] = useState(null);

    const submit = async (e) => {
        e.preventDefault();
        setSaving(true);
        setErr(null);
        try {
            // This endpoint must be implemented in BE: PUT /auth/profile { displayName, bio }
            const updated = await AuthApi.updateProfile({ displayName, bio });
            onSaved(updated);
        } catch (ex) {
            setErr(ex?.message || "Failed to save");
        } finally {
            setSaving(false);
        }
    };

    return (
        <form onSubmit={submit} className="space-y-4">
            <div className="flex justify-between items-center">
                <h3 className="text-lg font-semibold">Edit profile</h3>
                <button type="button" className="text-slate-500" onClick={onCancel}>Close</button>
            </div>

            {err && <div className="text-sm text-red-700">{err}</div>}

            <div>
                <label className="block text-sm text-slate-600">Display name</label>
                <input value={displayName} onChange={e => setDisplayName(e.target.value)} className="mt-1 w-full rounded-lg border px-3 py-2" />
            </div>

            <div>
                <label className="block text-sm text-slate-600">About / Bio</label>
                <textarea value={bio} onChange={e => setBio(e.target.value)} rows={4} className="mt-1 w-full rounded-lg border px-3 py-2" />
            </div>

            <div className="flex gap-3 justify-end">
                <button type="button" onClick={onCancel} className="rounded-lg px-4 py-2 border">Cancel</button>
                <button type="submit" disabled={saving} className="rounded-lg px-4 py-2 bg-ras-indigo text-white">{saving ? "Saving..." : "Save changes"}</button>
            </div>
        </form>
    );
}
