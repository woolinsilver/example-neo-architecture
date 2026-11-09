using Framework.Profiles;

namespace Mermaid.Tickets;

// The profile must implement this.
// For simplicity we say that IMermaidFeatures should be part of the profile, but it could be separate.

public interface IMermaidProfile : IProfile<MermaidTier>, IMermaidFeatures;
