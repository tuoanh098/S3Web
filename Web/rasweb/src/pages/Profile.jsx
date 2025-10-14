import React, { useEffect, useState } from "react";
import { useAuth } from "../auth/AuthContext";
import Avatar from "../ui/Avatar";
import { AuthApi, StudentsApi } from "../api"; // <- ensure StudentsApi exported here
import EditProfile from "./EditProfile";

export default function Profile() {
    const { user } = useAuth();
    const [profile, setProfile] = useState(null);
    const [loading, setLoading] = useState(true);
    const [editing, setEditing] = useState(false);
    const [err, setErr] = useState(null);

    useEffect(() => {
        let mounted = true;

        async function load() {
            setLoading(true);
            try {
                // 1) First try identity's /auth/me (may include .student)
                let me = null;
                try {
                    me = await AuthApi.me(); // your function; may return { identity, student } or just identity
                } catch (e) {
                    // if /auth/me requires auth header and user not logged in this may fail; ignore for now
                    me = null;
                }

                // If me contains student directly, use it
                if (mounted && me?.student) {
                    setProfile(me.student);
                    return;
                }

                // Determine the email to query with: prefer me.identity.email, else token claims (useAuth().user)
                const email =
                    me?.identity?.email ||
                    me?.email ||
                    user?.email ||
                    user?.unique_name ||
                    user?.sub; // fallback claims

                if (!email) {
                    // no email -> nothing to fetch
                    if (mounted) setProfile(null);
                    return;
                }

                // 2) Fallback: call Students service
                try {
                    const student = await StudentsApi.getByEmail(email);
                    if (mounted) setProfile(student);
                } catch (ex) {
                    // 404 or network error -> leave profile null but don't crash
                    if (mounted) setProfile(null);
                }
            } catch (ex) {
                if (mounted) setErr("Failed to load profile");
            } finally {
                if (mounted) setLoading(false);
            }
        }

        load();
        return () => (mounted = false);
    }, [user]);

    const onSaved = (updated) => {
        setProfile(updated);
        setEditing(false);
    };

    const displayName =
        profile?.displayName ||
        profile?.fullName ||
        user?.name ||
        user?.unique_name ||
        user?.email ||
        "RAS Student";

    const email = profile?.email || user?.email || "—";
    const roles = profile?.roles || user?.roles || user?.role || [];

    return (
        <div className="max-w-4xl mx-auto">
            <div className="bg-white rounded-2xl shadow p-6 md:p-10 grid grid-cols-1 md:grid-cols-3 gap-6">
                <div className="flex flex-col items-center md:items-start md:col-span-1">
                    <Avatar name={displayName} size={96} />
                    <h2 className="mt-4 text-xl font-bold text-ras-indigo">{displayName}</h2>
                    <p className="text-sm text-slate-500">
                        {roles && (Array.isArray(roles) ? roles.join(", ") : String(roles))}
                    </p>

                    <div className="mt-6 w-full">
                        <button onClick={() => setEditing(true)} className="w-full rounded-xl bg-ras-indigo text-white py-2">
                            Edit profile
                        </button>
                    </div>
                </div>

                <div className="md:col-span-2">
                    <h3 className="text-lg font-semibold text-slate-700 mb-4">Account details</h3>

                    {loading ? (
                        <div className="text-sm text-slate-500">Loading…</div>
                    ) : (
                        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                            <div>
                                <label className="block text-xs text-slate-500">Email</label>
                                <div className="mt-1 text-sm text-slate-800">{email}</div>
                            </div>

                            <div>
                                <label className="block text-xs text-slate-500">Joined</label>
                                <div className="mt-1 text-sm text-slate-800">
                                    {profile?.joinedAt ? new Date(profile.joinedAt).toLocaleString() : "—"}
                                </div>
                            </div>

                            <div className="md:col-span-2">
                                <label className="block text-xs text-slate-500">About</label>
                                <div className="mt-1 text-sm text-slate-800">{profile?.bio ?? "No profile description set."}</div>
                            </div>

                            <div className="md:col-span-2">
                                <label className="block text-xs text-slate-500">Actions</label>
                                <div className="mt-2 flex gap-2">
                                    <a href="/reset-password" className="text-sm text-ras-coral underline">
                                        Change password
                                    </a>
                                    <a
                                        href="#"
                                        onClick={(e) => {
                                            e.preventDefault();
                                            setErr("Export not implemented");
                                        }}
                                        className="text-sm text-slate-600"
                                    >
                                        Export data
                                    </a>
                                </div>
                            </div>
                        </div>
                    )}

                    {err && <div className="mt-4 text-sm text-red-700">{err}</div>}
                </div>
            </div>

            {/* Edit drawer/modal */}
            {editing && (
                <div className="fixed inset-0 z-40 flex items-end md:items-center justify-center bg-black/40 p-4">
                    <div className="bg-white w-full md:max-w-2xl rounded-xl shadow-lg p-6">
                        <EditProfile profile={profile} onSaved={onSaved} onCancel={() => setEditing(false)} />
                    </div>
                </div>
            )}
        </div>
    );
}
