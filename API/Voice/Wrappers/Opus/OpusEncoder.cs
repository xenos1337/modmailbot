﻿using System;
using System.Runtime.InteropServices;

namespace Discord.Voice
{
    public unsafe class OpusEncoder
    {
        protected IntPtr _ptr;

        public const int SamplingRate = 48000;
        public const int Channels = 2;
        public const int TimeBetweenFrames = 20;

        public const int SampleBytes = sizeof(short) * Channels;

        public const int FrameSamplesPerChannel = SamplingRate / 1000 * TimeBetweenFrames;
        public const int FrameSamples = FrameSamplesPerChannel * Channels;
        public const int FrameBytes = FrameSamplesPerChannel * SampleBytes;

        [DllImport("opus", EntryPoint = "opus_encoder_create", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr CreateEncoder(int Fs, int channels, int application, out OpusError error);
        [DllImport("opus", EntryPoint = "opus_encoder_destroy", CallingConvention = CallingConvention.Cdecl)]
        private static extern void DestroyEncoder(IntPtr encoder);
        [DllImport("opus", EntryPoint = "opus_encode", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Encode(IntPtr st, byte* pcm, int frame_size, byte* data, int max_data_bytes);
        [DllImport("opus", EntryPoint = "opus_encoder_ctl", CallingConvention = CallingConvention.Cdecl)]
        private static extern OpusError EncoderCtl(IntPtr st, OpusCtl request, int value);

        public AudioApplication Application { get; }
        public int BitRate { get; }

        public OpusEncoder(int bitrate, AudioApplication application, int packetLoss)
        {
            if (bitrate < 1 || bitrate > 384 * 1024)
                throw new ArgumentOutOfRangeException(nameof(bitrate));

            Application = application;
            BitRate = bitrate;

            OpusApplication opusApplication;
            OpusSignal opusSignal;
            switch (application)
            {
                case AudioApplication.Mixed:
                    opusApplication = OpusApplication.MusicOrMixed;
                    opusSignal = OpusSignal.Auto;
                    break;
                case AudioApplication.Music:
                    opusApplication = OpusApplication.MusicOrMixed;
                    opusSignal = OpusSignal.Music;
                    break;
                case AudioApplication.Voice:
                    opusApplication = OpusApplication.Voice;
                    opusSignal = OpusSignal.Voice;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(application));
            }

            _ptr = CreateEncoder(SamplingRate, Channels, (int)opusApplication, out var error);
            CheckError(error);
            CheckError(EncoderCtl(_ptr, OpusCtl.SetSignal, (int)opusSignal));
            CheckError(EncoderCtl(_ptr, OpusCtl.SetPacketLossPercent, packetLoss)); //%
            CheckError(EncoderCtl(_ptr, OpusCtl.SetInbandFEC, 1)); //True
            CheckError(EncoderCtl(_ptr, OpusCtl.SetBitrate, bitrate));
        }

        public unsafe int EncodeFrame(byte[] input, int inputOffset, byte[] output, int outputOffset)
        {
            int result = 0;
            fixed (byte* inPtr = input)
            fixed (byte* outPtr = output)
                result = Encode(_ptr, inPtr + inputOffset, FrameSamplesPerChannel, outPtr + outputOffset, output.Length - outputOffset);
            CheckError(result);
            return result;
        }

        private static void CheckError(int result)
        {
            if (result < 0)
                throw new Exception($"Opus Error: {(OpusError)result}");
        }

        private static void CheckError(OpusError error)
        {
            if (error < 0)
                throw new Exception($"Opus Error: {error}");
        }
    }
}
