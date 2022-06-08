using System.Collections.Generic;
using System.Text;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : Singleton<FPSCounter>
{
    [SerializeField] private Text text;

    [SerializeField] private Text profilerText;

    [SerializeField] private Text profilerValue;

    [SerializeField] private bool cpuProfiling = true;

    [SerializeField] private bool renderProfiling = true;

    [SerializeField] private bool memoryProfiling = true;

    [SerializeField] private float updateInterval = 0.5f;

    private float m_deltaFps;
    private int m_frames;
    private float m_timeleft;

    private struct Record
    {
        public ProfilerRecorder cpu;
        public ProfilerRecorder vsync;
        public ProfilerRecorder cameraRender;
        public ProfilerRecorder opaqueGeometry;
        public ProfilerRecorder transparentGeometry;
        public ProfilerRecorder imageEffects;
        public ProfilerRecorder drawCalls;
        public ProfilerRecorder trianglesCount;
        public ProfilerRecorder usedMemory;
        public ProfilerRecorder textureMemory;
    }

    private Record m_records;

    private void OnEnable()
    {
        m_timeleft = updateInterval;

        const int capacity = 15;
        m_records.cpu = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "PlayerLoop", capacity);
        m_records.vsync = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Gfx.WaitForRenderThread", capacity);
        m_records.cameraRender = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Camera.Render", capacity);
        m_records.opaqueGeometry =
            ProfilerRecorder.StartNew(ProfilerCategory.Render, "Render.OpaqueGeometry", capacity);
        m_records.transparentGeometry =
            ProfilerRecorder.StartNew(ProfilerCategory.Render, "Render.TransparentGeometry", capacity);
        m_records.imageEffects = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Camera.ImageEffects", capacity);
        m_records.drawCalls = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Draw Calls Count");
        m_records.trianglesCount = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Vertices Count");
        m_records.usedMemory = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "System Used Memory");
        m_records.textureMemory = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Texture Memory");
    }

    private void OnDisable()
    {
        m_records.cpu.Dispose();
        m_records.vsync.Dispose();
        m_records.cameraRender.Dispose();
        m_records.opaqueGeometry.Dispose();
        m_records.transparentGeometry.Dispose();
        m_records.imageEffects.Dispose();
        m_records.drawCalls.Dispose();
        m_records.trianglesCount.Dispose();
        m_records.usedMemory.Dispose();
        m_records.textureMemory.Dispose();
    }

    private void Update()
    {
        m_timeleft -= Time.deltaTime;
        m_deltaFps += Time.timeScale / Time.deltaTime;
        ++m_frames;

        if (m_timeleft <= 0f)
        {
            // display two fractional digits (f2 format)
            text.text = $"{m_deltaFps / m_frames:F1} fps";

            m_timeleft = updateInterval;
            m_deltaFps = 0f;
            m_frames = 0;

            // display profiler stats
            if (profilerText && profilerValue)
            {
                var stats = new StringBuilder(500);
                var values = new StringBuilder(500);

                if (cpuProfiling)
                {
                    if (m_records.cpu.Valid && m_records.vsync.Valid)
                    {
                        stats.AppendLine("CPU");
                        values.AppendLine($"{GetRecorderAvg(m_records.cpu) - GetRecorderAvg(m_records.vsync):F1} ms");
                    }
                }

                if (renderProfiling)
                {
                    stats.AppendLine("");
                    values.AppendLine("");

                    if (m_records.cameraRender.Valid)
                    {
                        stats.AppendLine("Render");
                        values.AppendLine($"{GetRecorderAvg(m_records.cameraRender):F1} ms");
                    }

                    if (m_records.opaqueGeometry.Valid)
                    {
                        stats.AppendLine("Opaque");
                        values.AppendLine($"{GetRecorderAvg(m_records.opaqueGeometry):F1} ms");
                    }

                    if (m_records.transparentGeometry.Valid)
                    {
                        stats.AppendLine("Transparent");
                        values.AppendLine($"{GetRecorderAvg(m_records.transparentGeometry):F1} ms");
                    }

                    if (m_records.imageEffects.Valid)
                    {
                        stats.AppendLine("Image Effects");
                        values.AppendLine($"{GetRecorderAvg(m_records.imageEffects):F1} ms");
                    }

                    if (m_records.drawCalls.Valid)
                    {
                        stats.AppendLine("Draw Calls");
                        values.AppendLine($"{m_records.drawCalls.LastValue:n0}");
                    }

                    if (m_records.trianglesCount.Valid)
                    {
                        stats.AppendLine("Vertices");
                        values.AppendLine($"{m_records.trianglesCount.LastValue:n0}");
                    }
                }

                if (memoryProfiling)
                {
                    stats.AppendLine("");
                    values.AppendLine("");

                    if (m_records.usedMemory.Valid)
                    {
                        stats.AppendLine("Used Memory");
                        values.AppendLine($"{BytesToMB(m_records.usedMemory.LastValue):F1} MB");
                    }

                    if (m_records.textureMemory.Valid)
                    {
                        stats.AppendLine("Texture Memory");
                        values.AppendLine($"{BytesToMB(m_records.textureMemory.LastValue):F1} MB");
                    }
                }

                profilerText.text = stats.ToString();
                profilerValue.text = values.ToString();
            }
        }
    }

    private static double GetRecorderAvg(ProfilerRecorder recorder, bool nsToMs = true)
    {
        var samplesCount = recorder.Count;

        if (samplesCount == 0)
            return 0;

        double r = 0;

        var samples = new List<ProfilerRecorderSample>(samplesCount);
        recorder.CopyTo(samples);
        for (var i = 0; i < samplesCount; ++i)
            r += samples[i].Value;

        r /= samplesCount;

        if (nsToMs)
        {
            return r * 1e-6f;
        }

        return r;
    }

    private static float BytesToMB(long bytes)
    {
        return bytes / (1024f * 1024f);
    }
}