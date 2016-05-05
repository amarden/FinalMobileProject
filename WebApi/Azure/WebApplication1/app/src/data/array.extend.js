Array.prototype.groupByAndSum = function (groupBy, unit) {
    var group = _.groupBy(this, groupBy);
    return _.map(group, function (g, key) {
        return {
            group: key,
            unit: pmdCalcHelpers.sum(g, unit)
        };
    });
};

Array.prototype.includeItems = function (list, item) {
    return _.filter(this, function (d) {
        return list.indexOf(d[item]) > -1;
    });
};

Array.prototype.filterZero = function (property) {
    return _.filter(this, function (item) {
        return item[property] > 0;
    });
};

Array.prototype.sortBy = function (prop) {
    return _.sortBy(this, function (d) { return -d[prop]; });
};
