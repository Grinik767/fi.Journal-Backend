using System.Text.RegularExpressions;
using Domain.Entities;
using Infrastructure.Repositories.Groups;
using Infrastructure.Repositories.Users;
using Group = Domain.Entities.Group;

namespace Application.Services.Groups;

public partial class GroupDistributionService(IGroupsRepository groupsRepository,
    IUsersRepository usersRepository) : IGroupDistributionService
{
    private static readonly Regex _studyGroupRegex = StudyGroupRegex();
    
    public async Task<User> DistributeUserIntoGroups(User user, CancellationToken ct)
    {
        var studyGroupData = GetStudyGroupData(user.StudyGroup);
        var groupsForUser = GetGroupsForUser(studyGroupData);
        var existingGroups = await groupsRepository.GetByNames(groupsForUser);
        var existingGroupsName = existingGroups.Select(g => g.Name).ToHashSet();
        foreach (var group in from groupName in groupsForUser where !existingGroupsName.Contains(groupName) select new Group(new Guid(), groupName))
        {
            group.AddUser(user);
            await groupsRepository.Add(group, ct);
        }

        foreach (var group in existingGroups)
        {
            group.AddUser(user);
            await groupsRepository.Update(group, new CancellationToken());
        }
        
        return user;
    }

    private List<string> GetGroupsForUser((int course, int group, int subgroup) studyGroupData)
    {
        var groups = new List<string>();
        groups.Add($"ФИИТ-{studyGroupData.course}");
        if (studyGroupData.course <= 2)
        {
            groups.Add($"ФТ-{studyGroupData.course}0{studyGroupData.group}");
            groups.Add($"ФТ-{studyGroupData.course}0{studyGroupData.group}-{studyGroupData.subgroup}");
            groups.Add(studyGroupData.group <= 2
                ? $"ФТ{studyGroupData.course}01-{studyGroupData.course}2"
                : $"ФТ{studyGroupData.course}03-{studyGroupData.course}4");
            return groups;
        }
        
        groups.Add("Курсы по выбору");
        return groups;
    }

    private (int course, int group, int subgroup) GetStudyGroupData(string studyGroup)
    {
        var match = _studyGroupRegex.Match(studyGroup);
        if (match.Success)
            return (
                int.Parse(match.Groups["course"].Value),
                int.Parse(match.Groups["group"].Value),
                int.Parse(match.Groups["subgroup"].Value)
            );
        return (-1, -1, -1);
    }

    [GeneratedRegex(@"ФТ-(?<course>\d)0(?<group>\d)-(?<subgroup>\d)")]
    private static partial Regex StudyGroupRegex();
}