export default function Dashboard() {
    return <div className="text-xl">Dashboard — basic KPIs coming soon.</div>;
}
//import React, { useEffect, useState } from "react";
//import { StudentsApi } from "../api";
//import { useNavigate } from "react-router-dom";

//export default function ProfileEdit() {
//    const nav = useNavigate();
//    const [loading, setLoading] = useState(true);
//    const [saving, setSaving] = useState(false);
//    const [error, setError] = useState(null);
//    const [form, setForm] = useState({
//        first_name: "",
//        last_name: "",
//        birthday: "",
//        gender: "",
//        nation: "",
//        mobile: "",
//        parent: "",
//        bio: ""
//    });

//    useEffect(() => {
//        let mounted = true;
//        StudentsApi.me()
//            .then(data => {
//                if (!mounted) return;
//                setForm({
//                    first_name: data.first_name ?? "",
//                    last_name: data.last_name ?? "",
//                    birthday: data.birthday ? data.birthday.split("T")[0] : "",
//                    gender: data.gender ?? "",
//                    nation: data.nation ?? "",
//                    mobile: data.mobile ?? "",
//                    parent: data.parent ?? "",
//                    bio: data.bio ?? ""
//                });
//            })
//            .catch(() => setError("Could not load profile"))
//            .finally(() => mounted && setLoading(false));

//        return () => { mounted = false; };
//    }, []);

//    function onChange(e) {
//        const { name, value } = e.target;
//        setForm(prev => ({ ...prev, [name]: value }));
//    }

//    async function onSubmit(e) {
//        e.preventDefault();
//        setError(null);
//        setSaving(true);

//        try {
//            const payload = {
//                first_name: form.first_name || null,
//                last_name: form.last_name || null,
//                birthday: form.birthday ? new Date(form.birthday).toISOString() : null,
//                gender: form.gender || null,
//                nation: form.nation || null,
//                mobile: form.mobile || null,
//                parent: form.parent || null,
//                bio: form.bio || null
//            };

//            await StudentsApi.updateMe(payload);
//            nav("/profile"); // back to profile view
//        } catch (err) {
//            console.error(err);
//            setError("Failed to update profile");
//        } finally {
//            setSaving(false);
//        }
//    }

//    if (loading) return <div className="p-6">Loading...</div>;

//    return (
//        <div className="max-w-3xl mx-auto p-6">
//            <h2 className="text-2xl font-semibold mb-4">Edit Profile</h2>

//            {error && <div className="mb-3 text-red-600">{error}</div>}

//            <form onSubmit={onSubmit} className="space-y-4">
//                <div>
//                    <label className="block text-sm">First name</label>
//                    <input name="first_name" value={form.first_name} onChange={onChange} className="w-full border p-2" />
//                </div>

//                <div>
//                    <label className="block text-sm">Last name</label>
//                    <input name="last_name" value={form.last_name} onChange={onChange} className="w-full border p-2" />
//                </div>

//                <div>
//                    <label className="block text-sm">Birthday</label>
//                    <input name="birthday" type="date" value={form.birthday} onChange={onChange} className="w-full border p-2" />
//                </div>

//                <div>
//                    <label className="block text-sm">Gender</label>
//                    <input name="gender" value={form.gender} onChange={onChange} className="w-full border p-2" />
//                </div>

//                <div>
//                    <label className="block text-sm">Nation</label>
//                    <input name="nation" value={form.nation} onChange={onChange} className="w-full border p-2" />
//                </div>

//                <div>
//                    <label className="block text-sm">Mobile</label>
//                    <input name="mobile" value={form.mobile} onChange={onChange} className="w-full border p-2" />
//                </div>

//                <div>
//                    <label className="block text-sm">Parent</label>
//                    <input name="parent" value={form.parent} onChange={onChange} className="w-full border p-2" />
//                </div>

//                <div>
//                    <label className="block text-sm">Bio</label>
//                    <textarea name="bio" value={form.bio} onChange={onChange} className="w-full border p-2" rows={5} />
//                </div>

//                <div className="flex gap-3">
//                    <button type="submit" className="px-4 py-2 bg-blue-600 text-white rounded" disabled={saving}>
//                        {saving ? "Saving..." : "Save"}
//                    </button>
//                    <button type="button" onClick={() => nav("/profile")} className="px-4 py-2 border rounded">Cancel</button>
//                </div>
//            </form>
//        </div>
//    );
//}
