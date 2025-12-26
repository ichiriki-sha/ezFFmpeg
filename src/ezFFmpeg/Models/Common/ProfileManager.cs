using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ezFFmpeg.Models.Common
{
    /// <summary>
    /// プロファイル管理クラス
    /// ObservableCollection を使用して WPF バインドに対応。
    /// プロファイルの追加・更新・削除・取得・デフォルト設定などを行う。
    /// </summary>
    public class ProfileManager : IEnumerable<Profile>
    {
        /// <summary>
        /// 管理しているプロファイル一覧
        /// </summary>
        public ObservableCollection<Profile> Profiles { get; private set; }

        // ============================
        // コンストラクタ
        // ============================
        /// <summary>
        /// ProfileManager のコンストラクタ
        /// 初期プロファイルを指定可能
        /// </summary>
        /// <param name="initialProfiles">初期プロファイルの列挙（省略可）</param>
        public ProfileManager(IEnumerable<Profile>? initialProfiles = null)
        {

            Profiles = initialProfiles != null
                ? new ObservableCollection<Profile>(initialProfiles)
                : [];
        }

        // ============================
        // 追加
        // ============================
        /// <summary>
        /// プロファイルを追加する
        /// 同一の ProfileId が存在する場合は例外
        /// </summary>
        /// <param name="profile">追加するプロファイル</param>
        public void Add(Profile profile)
        {
            ArgumentNullException.ThrowIfNull(profile);

            if (Profiles.Any(p => p.ProfileId == profile.ProfileId))
                throw new InvalidOperationException("同じ ProfileId のプロファイルが既に存在します");

            // デフォルトや最終使用フラグの一意性を維持
            foreach (var p in Profiles)
            {
                if (profile.IsDefault) p.IsDefault = false;
                if (profile.IsLastUsed) p.IsLastUsed = false;
            }

            Profiles.Add(profile);
        }

        // ============================
        // 更新（ProfileId 指定）
        // ============================
        /// <summary>
        /// プロファイルを更新する（ID は維持）
        /// </summary>
        /// <param name="profileId">更新対象の ProfileId</param>
        /// <param name="updatedProfile">更新内容</param>
        public void Update(Guid profileId, Profile updatedProfile)
        {
            ArgumentNullException.ThrowIfNull(updatedProfile);

            var existing = Profiles.FirstOrDefault(p => p.ProfileId == profileId)
                ?? throw new KeyNotFoundException($"Profile not found: {profileId}");

            existing.CopyFrom(updatedProfile);
        }

        // ============================
        // 削除
        // ============================
        /// <summary>
        /// プロファイルを ProfileId で削除する
        /// </summary>
        /// <param name="profileId">削除する ProfileId</param>
        /// <returns>削除できたか</returns>
        public bool Remove(Guid profileId)
        {
            var existing = Profiles.FirstOrDefault(p => p.ProfileId == profileId);
            if (existing == null) return false;
            return Profiles.Remove(existing);
        }

        /// <summary>
        /// プロファイルをインスタンスで削除する
        /// </summary>
        /// <param name="profile">削除するプロファイル</param>
        /// <returns>削除できたか</returns>
        public bool Remove(Profile profile)
        {
            if (profile == null) return false;
            return Remove(profile.ProfileId);
        }

        /// <summary>
        /// プロファイルをクリアする
        /// </summary>
        public void Clear()
        {
            Profiles.Clear(); 
        }

        // ============================
        // 取得
        // ============================
        /// <summary>
        /// ProfileId でプロファイルを取得
        /// 存在しない場合は例外
        /// </summary>
        /// <param name="profileId">取得する ProfileId</param>
        /// <returns>該当プロファイル</returns>
        public Profile Get(Guid profileId)
        {
            return Profiles.FirstOrDefault(p => p.ProfileId == profileId)
                   ?? throw new KeyNotFoundException($"Profile not found: {profileId}");
        }

        /// <summary>
        /// プロファイル名で取得（存在しなければ null）
        /// </summary>
        /// <param name="profileName">取得するプロファイル名</param>
        /// <returns>該当プロファイルまたは null</returns>
        public Profile? GetByName(string profileName)
        {
            return Profiles.FirstOrDefault(p => p.ProfileName == profileName);
        }

        // ============================
        // デフォルト取得
        // ============================
        /// <summary>
        /// デフォルト設定されているプロファイルを取得
        /// </summary>
        /// <returns>デフォルトプロファイルまたは null</returns>
        public Profile? GetDefault()
        {
            return Profiles.FirstOrDefault(p => p.IsDefault);
        }

        // ============================
        // 最終使用取得
        // ============================
        /// <summary>
        /// 最終使用プロファイルを取得
        /// </summary>
        /// <returns>最終使用プロファイルまたは null</returns>
        public Profile? GetLastUsed()
        {
            return Profiles.FirstOrDefault(p => p.IsLastUsed);
        }

        // ============================
        // デフォルト設定
        // ============================
        /// <summary>
        /// 指定プロファイルをデフォルトに設定（他は false に）
        /// </summary>
        /// <param name="profileId">設定する ProfileId</param>
        public void SetDefault(Guid profileId)
        {
            var target = Profiles.FirstOrDefault(p => p.ProfileId == profileId)
                ?? throw new InvalidOperationException($"Profile not found: {profileId}");

            if (target.IsDefault) return;

            foreach (var p in Profiles) p.IsDefault = false;

            target.IsDefault = true;
        }

        /// <summary>
        /// 指定プロファイルをデフォルトに設定（オブジェクト指定）
        /// </summary>
        /// <param name="profile">設定するプロファイル</param>
        public void SetDefault(Profile profile)
        {
            ArgumentNullException.ThrowIfNull(profile);
            SetDefault(profile.ProfileId);
        }

        /// <summary>
        /// 優先プロファイルを取得（最終使用→デフォルト→先頭）
        /// </summary>
        /// <returns>優先プロファイル</returns>
        public Profile? GetPriorityProfile()
        {
            var lastUsed = Profiles.FirstOrDefault(p => p.IsLastUsed);
            if (lastUsed != null) return lastUsed;

            var defaultProfile = Profiles.FirstOrDefault(p => p.IsDefault);
            if (defaultProfile != null) return defaultProfile;

            return Profiles.FirstOrDefault();
        }

        // ============================
        // 配列化
        // ============================
        /// <summary>
        /// プロファイルを配列で取得
        /// </summary>
        /// <returns>Profile 配列</returns>
        public Profile[] ToArray()
        {
            return [.. Profiles];
        }

        // IEnumerable<Profile> 実装
        public IEnumerator<Profile> GetEnumerator()
        {
            return Profiles.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
